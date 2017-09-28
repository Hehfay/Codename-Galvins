using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I think eventually the class hirearchy will be:
// Pickup -> CombatEquipment
// But for right now its not so useful to split.

public enum HandOccupancies {
    JustOneHanded,
    JustTwoHanded,
    OneOrTwoHanded,
    Armor,
};

public class Pickup : MonoBehaviour {
    // If the equipment is not active, it means the character
    // is not currently using this combat equipment.
    public bool active;
    public int inventoryIndex;
    public int count;

    void Start () {
        Debug.Assert (count != 0);
    }
}


