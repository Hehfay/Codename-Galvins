using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item: MonoBehaviour, IObjectData {

    public bool _stackable;
    public int _count;
    public string equipmentName;

    public int count () {
        return _count;
    }

    public void decreaseCount (int amount) {
        _count -= amount;
    }

    public void increaseCount (int amount) {
        _count += amount;
    }

    public string objectName () {
        return equipmentName;
    }

    public void setCount (int count) {
        _count = count;
    }

    public bool stackable () {
        return _stackable;
    }

    public string toolTip () {
        return equipmentName;
    }
}
