using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractUtility : MonoBehaviour {

    // The purpose of this function is to put things common to certain types of interaction
    // into one place.
    //
    // Things this fucntion handles:
    // - Lock character rotation
    // - Disable the Collider Interaction Controller
    //
    // Note that restricting player movement is not handled here, because its not common to all interactions.

    public static void InteractStart (GameObject Player) {
        Player.GetComponent<CursorManager> ().UnlockCursor ();
        Player.GetComponent<ColliderInteractController> ().DisableInteractController ();
    }

    public static void InteractEnd (GameObject Player) {
        Player.GetComponent<CursorManager> ().LockCursor ();
        Player.GetComponent<ColliderInteractController> ().EnableInteractController ();
    }
}