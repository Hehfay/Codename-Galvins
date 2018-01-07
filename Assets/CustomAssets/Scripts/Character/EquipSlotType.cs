using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EquipSlotType : ScriptableObject {

    public string slotName;
    // TODO: skeleton mapping data

    public override bool Equals(object other) {
        //return base.Equals(other);
        if (other != null && other.GetType() == typeof(EquipSlotType)) {
            EquipSlotType otherEquip = other as EquipSlotType;

            // This seems to be working.
            return GetInstanceID() == otherEquip.GetInstanceID(); // if ids are the same, then they are the same object
        }
        return false; // not of same object type, so can't be same object
    }
}