using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuallyHurt : MonoBehaviour
{

    public float range = 5.0f;
    public float damageRate = 10.0f;

    private float prop = 1.0f;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
    }


    private float CalcDamage(Collider col) {
        float d = Time.deltaTime * damageRate * (1.0f - Vector3.Distance(col.transform.position, transform.position) / range); ;
        return prop * ((d > 0.0f) ? d : 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        prop = 1.1f / (1.0f + Time.time - startTime) - 0.1f;
        if (prop < 0.0f) {
            Destroy(this.gameObject);
        } else {
            foreach (Collider col in Physics.OverlapSphere(transform.position, range)) {
                if (col.gameObject.GetComponent<Enemy>()) {
                    col.gameObject.GetComponent<Enemy>().Damage(CalcDamage(col));
                } else if (col.gameObject.GetComponent<PlayerScript>()) {
                    col.gameObject.GetComponent<PlayerScript>().Damage(CalcDamage(col));
                }
            }
        }

    }
}
