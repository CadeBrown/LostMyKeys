using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFacing : MonoBehaviour
{

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        if (!target) target = GameObject.FindWithTag("Player").GetComponentInChildren<Camera>().transform; 
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
    }
}
