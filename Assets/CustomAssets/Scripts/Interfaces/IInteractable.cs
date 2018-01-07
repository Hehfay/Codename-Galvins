using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
    bool Interact (GameObject gameobject);
    string ToolTip ();
}