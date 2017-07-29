using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is on the first person controller script and
// handles toggling the UI.
public class UIManager : MonoBehaviour {

    InventoryController invCont;

    GameObject canvas;
    bool isActive;

    Character myCharacterScript;

	void Start () {

        myCharacterScript = GetComponent<Character> ();

        invCont = GameObject.Find ("InventoryController").GetComponent<InventoryController>();
        canvas = GameObject.Find ("Canvas");

        Debug.Assert (canvas != null);
        Debug.Assert (invCont != null);

        isActive = false;
        canvas.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            // Toggle the inventory display.
            isActive = !isActive;
            myCharacterScript.allowedToPickThingsUp = !isActive;
            if (isActive) {
                invCont.readFromCharacterToInventory ();
            }
            else {
                invCont.deleteUiElements ();
            }
            canvas.SetActive (isActive);
        }
	}
}