using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInteraction : MonoBehaviour, IInteractable {

    public string characterName;

    public bool Interact (GameObject gameobject) {
        // Draw the dialog box.
        GetComponent<UIDialogFactory> ().CreateFactoryItem ();
        GetComponent<UIDialogFactory> ().enabled = true;

        gameobject.GetComponent<CursorManager> ().enabled = false;
        gameobject.GetComponent<PlayerMovementController> ().enabled = false;
        gameobject.GetComponent<CursorManager> ().UnlockCursor ();
        gameobject.GetComponent<CursorManager> ().enabled = false;
        gameobject.GetComponent<ColliderInteractController> ().DestroyPopUpConditionally ();
        gameobject.GetComponent<ColliderInteractController> ().enabled = false;

        // Disable factory input.
        gameobject.GetComponent<UIInputHandler> ().enabled = false;

        // Create the inventory.
        return true;
    }

    public string ToolTip () {
        return characterName;
    }
}
