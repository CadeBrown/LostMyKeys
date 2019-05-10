using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LavaPit : MonoBehaviour
{

    public TextMeshProUGUI message;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<PlayerScript>()) {
            PlayerScript ps = other.gameObject.GetComponent<PlayerScript>();

            int numKeys = 0; 
            ps.inventory.TryGetValue(ItemID.KEY_GREEN, out numKeys);
            if (numKeys >= 3) {
                // they 'win'
                message.SetText("Did you really think that jumping in lava with a certain number of keys wouldn't kill you?");
            } else {
                // they lose
                message.SetText("You need 3 keys, I don't make the rules! You just burned up!");
            }

            ps.Damage(1000.0f);
        }
    }
}
