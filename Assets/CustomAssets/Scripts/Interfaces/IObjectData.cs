using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObjectData {
    string objectName ();
    string toolTip ();
    bool stackable ();
    void setCount (int count);
    int count();
    void increaseCount (int amount);
    void decreaseCount (int amount);
}