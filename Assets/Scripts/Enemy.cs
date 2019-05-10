using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 3.0f;
    public float jumpThreshold = 4.0f;
    public float jumpStrength = 3.0f;
    public float range = 4.0f;
    public float activationRange = 20.0f;
    public float damagePerSecond = 10.0f;
    public float delayUntilFirst = 0.5f;
    public float delayEachAttack = 1.0f;
    public float attackAnimationPredelay = 0.2f;

    public float health = 25.0f;
    public float maxHealth = 25.0f;

    public Transform attackRoot;

    private GameObject _player;
    private Animator animator;
    private float _nextAttack = -1.0f;
    private int _curCombo = -1;
    private Rigidbody _rb;
    public bool isDead = false;
    private float tryToJump = -1.0f;
    public bool isFollowing = false;

    private float distToGround;

    void Start() {
        _player = GameObject.FindWithTag("Player");
        animator = GetComponent<Animator>();
        distToGround = GetComponent<Collider>().bounds.extents.y;
        _rb = GetComponent<Rigidbody>();
    }

    bool isGrounded() {
        return Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);

    }

    bool canSeeTarget() {
        RaycastHit hit;
        return Physics.Raycast(attackRoot.position, _player.transform.position - attackRoot.position, out hit, 1.05f * range) && hit.transform.gameObject == _player;
    }

    public void Die() {
        if (!isDead) {
            isDead = true;

            animator.SetFloat("speed", 0.0f);
            animator.SetTrigger("die");
            //Destroy(_rb);
            Destroy(this.gameObject, 7.5f);
        }
    }

    public void Damage(float amt, Vector3 dir=new Vector3()) {
        health -= amt;
        if (health <= 0.0f) Die();
        if (amt > 4.0f && dir.magnitude > 0.01f) {
            // apply a lil force
            dir = dir.normalized;
            dir.y = 0.5f;
            if (health <= 0.0f) {
                _rb.AddForce(2.1f * dir.normalized * (0.4f + 0.6f * Mathf.Clamp(amt - 4.0f, 0.0f, 46.0f) / 46.0f), ForceMode.Impulse);
            } else {
                _rb.AddForce(2.1f * dir.normalized * (0.25f + 0.75f * Mathf.Clamp(amt - 4.0f, 0.0f, 46.0f) / 46.0f), ForceMode.Impulse);
            }
        }
    }

    // called by the player
    void Update() {
        if (Time.timeScale > 0.0f && !isDead) {

            Vector3 dir = _player.transform.position - attackRoot.position;
            transform.rotation = Quaternion.LookRotation(new Vector3(dir.x, 0.0f, dir.z));

            if (dir.magnitude <= activationRange) {
                isFollowing = true;
            }

            if (isFollowing) {


                if (dir.magnitude > range) {
                    transform.position += transform.forward * Time.deltaTime * speed;
                    animator.SetFloat("speed", speed);
                } else {
                    animator.SetFloat("speed", 0.0f);
                }

                // jump
                if (dir.y > jumpThreshold && isGrounded()) {
                    if (tryToJump < 0.0f) {
                        tryToJump = Time.time;
                    }
                    if (Time.time - tryToJump > 1.0f) {
                        animator.SetTrigger("jump");
                        _rb.velocity += Vector3.up * jumpStrength * (0.12f + 0.88f * Mathf.Clamp(dir.y - jumpThreshold, 0.0f, 6.0f) / 6.0f);
                    }
                } else {
                    tryToJump = -1.0f;
                }

                if (dir.magnitude < 1.05f * range && isGrounded() && canSeeTarget()) {
                    // attacking
                    if (_nextAttack < 0.0f) {
                        _nextAttack = Time.time + delayUntilFirst;
                        _curCombo = 0;
                    } else if (Time.time > _nextAttack) {
                        _player.GetComponent<PlayerScript>().Damage(delayEachAttack * damagePerSecond);
                        _curCombo++;
                        _nextAttack += delayEachAttack;
                        animator.SetTrigger("attack");
                    }
                } else {
                    _nextAttack = -1.0f;
                }
            }
        }
    }
}
