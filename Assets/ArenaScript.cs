using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaScript : MonoBehaviour
{

    public Transform door;
    public float yChange = 2.0f;
    public float ySnap = 1.0f;
    public float xOff = -4.0f;

    public GameObject key;

    public int wavesize = 5;

    // enemies and chance of spawning
    public GameObject[] enemies;
    public float[] weights;

    public GameObject[] rewards;
    public float[] rewards_weights;

    public bool isLocked = false;

    public int numKilled = 0;

    private bool lastIsLocked = false;

    private Vector3 ogPos;

    private List<GameObject> spawned;
    private BoxCollider _coll;

    // Start is called before the first frame update
    void Start()
    {
        ogPos = door.position;
        // normalize
        float t = 0.0f;
        foreach (float f in weights) t += f;
        t /= weights.Length;
        _coll = GetComponent<BoxCollider>();
        foreach (float f in rewards_weights) t += f;
        t /= rewards_weights.Length;

        _coll = GetComponent<BoxCollider>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isLocked) {
            door.position = Vector3.Lerp(door.position, ogPos + Vector3.up * ((isLocked) ? yChange : 0.0f), Time.deltaTime * ySnap);
        } else {
            door.position = Vector3.Lerp(door.position, ogPos + Vector3.up * ((isLocked) ? yChange : 0.0f), Time.deltaTime * ySnap / 2.5f);

        }
        if (isLocked) {
            // be spawning enemies
            if (!lastIsLocked) {
                // reset counts
                spawned = new List<GameObject>();
            }
            numKilled = 0;

            foreach (GameObject fi in spawned) {
                if (fi && fi.GetComponent<Enemy>().isDead) {
                    numKilled++;
                } else if (!fi) {
                    numKilled++;
                }
            }


            if (numKilled < wavesize && spawned.Count < wavesize * 5 / 3) {
                // maybe spawn another one
                if (Random.Range(0.0f, Time.deltaTime) < Time.deltaTime / (2.0f * Time.deltaTime + 2.0f)) {
                    float rnd = Random.value;
                    float acc = 0.0f;
                    int i = 0;
                    while (acc < rnd && i < weights.Length) {
                        acc += weights[i];
                        ++i;
                    }
                    --i;
                    GameObject newObj = Instantiate(enemies[i]);
                    newObj.SetActive(true);
    
                                    newObj.transform.position = new Vector3(
                            Random.Range(_coll.bounds.min.x, _coll.bounds.max.x), Random.Range(0.1f, _coll.bounds.max.y), Random.Range(_coll.bounds.min.z, _coll.bounds.max.z));
                    // spawn another one
                    spawned.Add(newObj);
                }
            } else if (numKilled >= wavesize) {
                // end it
                isLocked = false;
                foreach (GameObject fi in spawned) {
                    if (fi) fi.GetComponent<Enemy>().Damage(1000.0f);
                }
                float rw = Random.value;
                float acc = 0.0f;
                int i = 0;
                while (acc < rw && i < rewards_weights.Length) {
                    acc += rewards_weights[i];
                    ++i;
                }
                --i;
                GameObject newKey = Instantiate(rewards[i]);
                newKey.transform.position = door.transform.position + Vector3.forward * xOff + Vector3.up * 2.0f;
            }
        }

        lastIsLocked = isLocked;

    }

    private void OnTriggerEnter(Collider other) {
        // when player enters
        if (other.gameObject.tag == "Player") {
            isLocked = true;
        }
    }

}
