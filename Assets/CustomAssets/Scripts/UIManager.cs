using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is on the first person controller script and
// handles toggling the UI.
public class UIManager : MonoBehaviour {

    InventoryController invCont;

    GameObject lefthand;
    GameObject righthand;
    GameObject inventory;
    bool isActive;

    Character myCharacterScript;

	void Start () {

        myCharacterScript = GetComponent<Character> ();

        invCont = GameObject.Find ("InventoryController").GetComponent<InventoryController>();
        lefthand = GameObject.Find ("LeftHand");
        righthand = GameObject.Find ("RightHand");
        inventory = GameObject.Find ("Inventory");

        Debug.Assert (lefthand != null);
        Debug.Assert (righthand != null);
        Debug.Assert (inventory != null);
        Debug.Assert (invCont != null);

        isActive = false;
        lefthand.SetActive (false);
        righthand.SetActive (false);
        inventory.SetActive (false);
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
            lefthand.SetActive (isActive);
            righthand.SetActive (isActive);
            inventory.SetActive (isActive);
        }
	}
}