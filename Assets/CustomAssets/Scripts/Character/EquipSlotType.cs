using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipSlotType : ScriptableObject {

    private System.Guid id;
    string name;
    // TODO: skeleton mapping data

    public override bool Equals(object other) {
        //return base.Equals(other);
        if (other != null && other.GetType() == typeof(EquipSlotType)) {
            EquipSlotType otherEquip = other as EquipSlotType;
            return id == otherEquip.id; // if ids are the same, then they are the same object
        }
        return false; // not of same object type, so can't be same object
    }

}
