using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Door : MonoBehaviour {

    public float angleToRotate = 80.0f;
    public float rotateSpeed = 2.0f;
    public bool closed = false;

    public ItemID itemRequired = ItemID.KEY_GREEN;

    public bool isLocked = true;

    public AudioSource sfxSource;
    public AudioClip audioUnlock, audioOpen, audioClose;

    private float _startAngle;

    // Start is called before the first frame update
    void Start() {
        _startAngle = transform.eulerAngles.y;
    }

    public void Toggle() {
        if (!isLocked) {
            closed = !closed;
        }
        if (closed) {
            sfxSource.PlayOneShot(audioClose);
        } else {
            sfxSource.PlayOneShot(audioOpen);
        }
    }

    public void Unlock() {
        if (isLocked) {
            isLocked = false;
        }
        sfxSource.PlayOneShot(audioUnlock);
    }


    // Update is called once per frame
    void Update() {
        float targetAngle;
        if (closed) {
            targetAngle = _startAngle;
        } else {
            targetAngle = angleToRotate + _startAngle;
        }

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotateSpeed), transform.eulerAngles.z);
    }
}
