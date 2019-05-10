using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : MonoBehaviour
{
    public float health = 10.0f;

    public GameObject drop;

    public int fragmentSize = 12;

    public void Damage(float amt) {
        health -= amt;
        if (health < 0.0f) {
            gameObject.AddComponent<TriangleExplosion>();
            gameObject.GetComponent<TriangleExplosion>().fragmentSize = fragmentSize;

            StartCoroutine(gameObject.GetComponent<TriangleExplosion>().SplitMesh(true, 2.0f));
            if (drop) {
                GameObject nw = Instantiate(drop);
                nw.transform.position = transform.position + Vector3.up;
            }
            Destroy(this.gameObject, 0.1f);
        }
    }
}
