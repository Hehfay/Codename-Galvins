using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Sign: MonoBehaviour, IInteractable {

    public string SignText;

    public bool Interact (GameObject gameobject) {
        return true;
    }

    public string ToolTip () {
        return SignText;
    }

}
