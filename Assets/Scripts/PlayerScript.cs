using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour {

    public float moveSpeedSideways = 12.0f, moveSpeedForwards = 12.0f;
    public float turnSpeedHorizontal = 1.0f, turnSpeedVertical = 1.0f;

    public float jumpStrength = 20.0f;

    public float fallThreshold = 4.0f;
    public float fallDamage = 5.0f;

    public float gravity = 16.0f;

    public float health = 100.0f, maxHealth = 100.0f;

    public float flinchAmount;

    public Image reticleImage;
    public Sprite reticleDefault, reticlePickup, reticleDoorInteract, reticleDoorInteract_needKey, reticleDoorInteract_haveKey;

    public HealthBar healthBar;

    public Weapon[] weapons;

    public Dictionary<ItemID, int> inventory = new Dictionary<ItemID, int>();

    // UI components
    public TextMeshProUGUI hoverInfoName, hoverInfoDesc;
    public GameObject keyCountGreen /*, keyCountBlue, keyCountRed */;

    public AudioSource sfxSource;

    public AudioClip audioPickup, audioHurt, audioDie;

    private float flinchDurLeft = 0.0f;

    // private things to keep track of
    private float _yVel = 0.0f;

    private int curWeapon = 0;

    // shortcut references
    private CharacterController _cc;
    private Camera _camera;
    private TextMeshProUGUI keyCountTextGreen, keyCountTextBlue, keyCountTextRed;
    private MenuScript _menuScript;
    private Vector3 _cameraOriginalPosition;
    private float _fallStartY;
    private bool isFalling = false;
    private bool sliding = false;
    private Vector3 slidingDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start() {
        _cc = GetComponent<CharacterController>();
        _menuScript = GetComponent<MenuScript>();
        _camera = GetComponentInChildren<Camera>();
        keyCountTextGreen = keyCountGreen.GetComponentInChildren<TextMeshProUGUI>();
        //keyCountTextBlue = keyCountBlue.GetComponentInChildren<TextMeshProUGUI>();
        //keyCountTextRed = keyCountRed.GetComponentInChildren<TextMeshProUGUI>();
        inventory.Add(ItemID.KEY_GREEN, 0);
        inventory.Add(ItemID.WEAPON_BAREHANDS, 1);
        inventory.Add(ItemID.WEAPON_FLASHLIGHT, 1);
        inventory.Add(ItemID.WEAPON_KATANA, 0);
        inventory.Add(ItemID.WEAPON_AK47, 0);
        inventory.Add(ItemID.WEAPON_LASERRIFLE, 0);
        inventory.Add(ItemID.WEAPON_FLAMETHROWER, 1);
        Time.timeScale = 1.0f;

        curWeapon = 3;
        _cameraOriginalPosition = _camera.gameObject.transform.localPosition;
        int i;
        for (i = 0; i < weapons.Length; ++i) {
            weapons[i].gameObject.SetActive(i == curWeapon);
        }
    }

    Weapon GetWeapon() {
        return weapons[curWeapon];
    }

    void SelectWeapon() {
        int startWeapon = curWeapon;
        GetWeapon().gameObject.SetActive(false);

        do {
            curWeapon = (curWeapon + 1) % weapons.Length;
        } while (inventory[weapons[curWeapon].requiredItemID] < 1 && curWeapon != startWeapon);

        GetWeapon().gameObject.SetActive(true);

    }

    void SelectWeapon(ItemID id) {
        int startWeapon = curWeapon;
        GetWeapon().gameObject.SetActive(false);

        do {
            curWeapon = (curWeapon + 1) % weapons.Length;
        } while (weapons[curWeapon].requiredItemID != id && curWeapon != startWeapon);

        GetWeapon().gameObject.SetActive(true);

    }

    private void SetReticle(Sprite rt, Color cl) {
        reticleImage.sprite = rt;
        reticleImage.color = cl;
    }

    private void SetHoverInfo() {
        hoverInfoName.SetText("");
        hoverInfoDesc.SetText("");
    }

    private void SetHoverInfo(Interaction hi) {
        hoverInfoName.SetText(hi.title);
        hoverInfoDesc.SetText(hi.description);
    }

    public void Damage(float amt) {
        health -= amt;
        if (amt > 5.0f) {
            if (health > 0.0f) sfxSource.PlayOneShot(audioHurt);
            flinchDurLeft += amt / 20.0f;
        }
        if (health <= 0.0f) {
            Die();
        }
    }

    public void Heal(float amt) {
        health += amt;
    }


    public void Die() {
        sfxSource.PlayOneShot(audioDie);

        _menuScript.Pause();
        _menuScript.ShowDeathScreen();
    }

    void OnControllerColliderHit(ControllerColliderHit hit) {
        if (hit.normal.y < Mathf.Sin(_cc.slopeLimit * Mathf.Deg2Rad)) {
            sliding = true;

            Vector3 normal = hit.normal;
            Vector3 c = Vector3.Cross(Vector3.up, normal);
            Vector3 u = Vector3.Cross(c, normal);
            slidingDirection = u * 4f;

        } else {
            sliding = false;
            slidingDirection = Vector3.zero;
        }
    }


    // Update is called once per frame
    void Update() {
        // change weapon
        if (Input.GetKeyDown(KeyCode.E)) {
            SelectWeapon();
        }

        if (Input.GetAxisRaw("Fire") > 0.5f) {
            if (curWeapon >= 0 && curWeapon < weapons.Length && weapons[curWeapon]) weapons[curWeapon].Attack(_camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)));
        }

        if (!_cc.isGrounded) {
            if (!isFalling) {
                _fallStartY = transform.position.y;
            }
            isFalling = true;
        } else {
            if (isFalling) {
                float diffY = _fallStartY - transform.position.y;
                if (diffY > fallThreshold) {
                    Damage((diffY - fallThreshold) * fallDamage);
                }
            }
            isFalling = false;
        }


        // setting the hovering information
        RaycastHit hit;
        if (Physics.Raycast(_camera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0)), out hit, 12.0f)) {
            Interaction _hoverInfo;
            if (_hoverInfo = hit.transform.gameObject.GetComponent<Interaction>()) {
                bool interacting = Input.GetKeyDown(KeyCode.F);
                SetHoverInfo(_hoverInfo);

                if (_hoverInfo.interactionType == Interaction.Type.PICKUP) {
                    SetReticle(reticlePickup, Color.white);

                    if (interacting) {
                        // TODO Pick up item
                        int count = 0;
                        inventory.TryGetValue(_hoverInfo.ID, out count);
                        inventory[_hoverInfo.ID] = count + 1;

                        // autoequip
                        //if (_hoverInfo.ID == ItemID.WEAPON_KATANA) {
                            SelectWeapon(_hoverInfo.ID);
                        //}

                        Destroy(_hoverInfo.gameObject);
                        sfxSource.PlayOneShot(audioPickup);
                    }

                } else if (_hoverInfo.interactionType == Interaction.Type.DOORINTERACT) {
                    Door door = _hoverInfo.gameObject.GetComponentInParent<Door>();
                    if (door.isLocked) {
                        // check how to open it
                        int count = 0;
                        inventory.TryGetValue(door.itemRequired, out count);
                        if (count > 0) {
                            if (interacting) {
                                inventory[door.itemRequired] = count - 1;
                                door.Unlock();
                            }
                            SetReticle(reticleDoorInteract_haveKey, Color.white);
                        } else {
                            SetReticle(reticleDoorInteract_needKey, Color.white);
                        }
                    }

                    // seperate 'if' in case it gets unlocked
                    if (!door.isLocked) {
                        SetReticle(reticleDoorInteract, Color.white);
                        if (interacting) {
                            door.Toggle();
                        }
                    }
                } else if (_hoverInfo.interactionType == Interaction.Type.ENEMY) {
                    SetReticle(reticleDefault, Color.red);
                }
            } else {
                SetHoverInfo();
                SetReticle(reticleDefault, Color.white);
            }
        } else {
            SetHoverInfo();
            SetReticle(reticleDefault, Color.white);
        }

        healthBar.setProportion(health / maxHealth);

        int ct = 0;
        inventory.TryGetValue(ItemID.KEY_GREEN, out ct);
        keyCountTextGreen.SetText(ct.ToString());

        Vector3 tMoveForward = transform.forward;
        tMoveForward.y = 0.0f;
        tMoveForward.Normalize();
        tMoveForward *= moveSpeedForwards * (Input.GetAxisRaw("Forward") - Input.GetAxisRaw("Back"));
        Vector3 tMoveSideways = transform.right;
        tMoveSideways.y = 0.0f;
        tMoveSideways.Normalize();
        tMoveSideways *= moveSpeedSideways * (Input.GetAxisRaw("Right") - Input.GetAxisRaw("Left"));


        _yVel -= Time.deltaTime * gravity;

        if (Input.GetAxisRaw("Jump") > 0.5f && _cc.isGrounded && !sliding) {
            _yVel = jumpStrength;
        } else if (_cc.isGrounded) {
            _yVel = 0.0f;
        }

        float tRotateHorizontal = turnSpeedHorizontal * Input.GetAxisRaw("Mouse X");
        float tRotateVertical = turnSpeedVertical * -Input.GetAxisRaw("Mouse Y");

        _cc.Move(Time.deltaTime * (tMoveForward + slidingDirection + Vector3.up * _yVel + tMoveSideways));
        _camera.gameObject.transform.Rotate(Time.deltaTime * new Vector3(tRotateVertical, 0.0f, 0.0f));
        transform.Rotate(Time.deltaTime * new Vector3(0.0f, tRotateHorizontal, 0.0f));

        float viewAngle = 80.0f;
        float pitch = _camera.transform.eulerAngles.x;

        if (pitch >= viewAngle && pitch < 180.0f) {
            pitch = viewAngle;
        } else if (pitch <= 360.0f - viewAngle && pitch > 180.0f) {
            pitch = 360.0f - viewAngle;
        }

        _camera.transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);

        if (flinchDurLeft > 0.0f) {
            _camera.gameObject.transform.localPosition = _cameraOriginalPosition + Random.insideUnitSphere * flinchAmount * flinchDurLeft;
            flinchDurLeft -= Time.deltaTime;
            if (flinchDurLeft < 0.0f) {
                flinchDurLeft = 0.0f;
            }
        }
    }
}
