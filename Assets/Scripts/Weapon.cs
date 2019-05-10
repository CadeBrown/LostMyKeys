
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Weapon : MonoBehaviour {

    public ItemID requiredItemID;

    public GameObject projectile;
    public bool useProjectile = false;

    public float fireRate = 1.0f;
    public float range = 5.0f;
    public float damageAmount = 15.0f;

    public Transform projectileSource;
    public AudioSource sfxSource;
    public AudioClip audioAttack;

    private Animator animator;
    private float _lastFire;
    private LineRenderer _lineRenderer;

    void Start() {
        animator = GetComponent<Animator>();
        if (!animator) animator = GetComponentInChildren<Animator>();
        _lastFire = -1.0f;
        _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer) _lineRenderer.widthMultiplier = 0.0f;
    }

    void Update() {
        //_lineRenderer.SetPosition(0, projectileSource.position);
        if (_lineRenderer) _lineRenderer.widthMultiplier *= 0.8f;
    }

    private void AttackMelee(Ray attackRay) {
        RaycastHit hit;
        if (Physics.Raycast(attackRay, out hit, range)) {
            GameObject ob = hit.transform.gameObject;
            if (ob.GetComponent<Enemy>()) {
                ob.GetComponent<Enemy>().Damage(damageAmount, attackRay.direction);
            } else if (ob.GetComponent<Shatter>()) {
                ob.GetComponent<Shatter>().Damage(damageAmount);
            }

        } else {
            // failed to actually hit anything 

        }
    }

    private void AttackProjectile(Ray attackRay) {
        GameObject t = Instantiate(projectile);
        t.transform.position = projectileSource.position + attackRay.direction * 0.5f;
        t.GetComponent<Rigidbody>().velocity = attackRay.direction.normalized * 40.0f;
        t.SetActive(true);
    }

    public bool Attack(Ray attackRay) {
        if (Time.time >= _lastFire + 1.0f / fireRate) {

            // actually attack
            animator.SetTrigger("attack");
            if (sfxSource) sfxSource.PlayOneShot(audioAttack);

            _lastFire = Time.time;

            if (useProjectile) {
                AttackProjectile(attackRay);
            } else {
                AttackMelee(attackRay);
            }

            if (_lineRenderer) {
                _lineRenderer.SetPosition(0, projectileSource.position);
                _lineRenderer.SetPosition(1, attackRay.origin + attackRay.direction * range);
                _lineRenderer.widthMultiplier = 1.0f;
            }

            return true;
        } else {
            return false;
        }
    }
}
