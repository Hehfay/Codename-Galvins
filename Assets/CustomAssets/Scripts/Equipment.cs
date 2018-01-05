using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour, IObjectData, IEquipable {

    public bool _stackable;
    public int _count;
    public string equipmentName;
    public EquipSlotType equipmentType;

    public int count () {
        return _count;
    }

    public void setCount (int count) {
        _count = count;
    }

    public EquipSlotType GetEquipSlotType () {
        return equipmentType;
    }

    public string objectName () {
        return equipmentName;
    }

    public bool stackable () {
        return _stackable;
    }

    public string toolTip () {
        return equipmentName;
    }

    public void increaseCount (int amount) {
        _count += amount;
    }

    public void decreaseCount (int amount) {
        _count -= amount;
    }
}