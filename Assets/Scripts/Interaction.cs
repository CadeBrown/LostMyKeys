using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour {

    public enum Type {
        NOTHING,
        PICKUP,
        DOORINTERACT,
        ENEMY
    };

    public string title;
    public string description;
    public Type interactionType;
    public ItemID ID;

}
