using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;

// This script handles the click event for UI elements.
// When a UI element is clicked on, it is dropped.
public class UiClickHandler : MonoBehaviour, IPointerClickHandler {

    public GameObject dropItemPrefab;

    public GameObject UiElementSelectCountPrefab;

    string playerString = "Player(Clone)";

    // This needs to be static because UI element has its own
    // UiClickHandler, which maybe is bad.
    public static bool countButtonEnabled;

    public static UiClickHandler currentClickHandler;

    public void Start () {
        countButtonEnabled = false;
    }

    public void OnPointerClick (PointerEventData eventData) {

        InventoryController i = GameObject.Find ("Player(Clone)").GetComponent<InventoryController>();

        Character character = GameObject.Find (playerString).GetComponent<Character>();

        int index = gameObject.GetComponent<Pickup> ().inventoryIndex;

        int itemCount;

        Transform t = gameObject.transform.parent.transform.parent;

        if (t == i.rh.transform) {
            itemCount = character.rightHandItemCount[index];
        }
        else if (t == i.lh.transform) {
            itemCount = character.leftHandItemCount[index];
        }
        else {
            itemCount = character.itemCount[index];
        }

        if (countButtonEnabled) {
            return;
        }

        if (itemCount > 1) {
            GameObject button = Instantiate (UiElementSelectCountPrefab) as GameObject;

            // The parent of this object is the canvas. Seems fine but the statement is ugly.
            button.transform.SetParent (gameObject.transform.parent.transform.parent.transform.parent.transform);
            button.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
            button.GetComponentInChildren<Text> ().text = "1";

            button.GetComponentInChildren<PlusButtonOnClick> ().maxCount = GetComponent<Pickup> ().count;

            currentClickHandler = this;
            countButtonEnabled = true;
            i.DisableDragHandlers ();
            return;
        }

        // If you are trying to drop a non-stackable itme this logic will run.
        GameObject g = Instantiate (dropItemPrefab) as GameObject;
        g.GetComponent<Pickup>().pickupData = gameObject.GetComponent<Pickup>().pickupData;

        GameObject c = GameObject.Find (playerString);
        g.transform.position = c.transform.position;

        gameObject.transform.SetParent (null);
        gameObject.GetComponent<Pickup> ().count--;
        Destroy (gameObject);
        // character.itemCount[index]--;
        i.readFromInventoryToCharacter ();
        i.UpdateGuiCounts ();
    }

    public void DropStackOfItems (int dropCount) {
        InventoryController i = GameObject.Find ("Player(Clone)").GetComponent<InventoryController>();

        GameObject g = Instantiate (dropItemPrefab) as GameObject;
        g.GetComponent<Pickup>().pickupData = gameObject.GetComponent<Pickup>().pickupData;

        GameObject c = GameObject.Find (playerString);
        g.transform.position = c.transform.position;

        //character.itemCount[index]--;
        gameObject.GetComponent<Pickup> ().count -= dropCount;
        g.GetComponent<Pickup> ().count = dropCount;

        if (gameObject.GetComponent<Pickup>().count == 0) {
            gameObject.transform.SetParent (null);
            Destroy (gameObject);
        }

        i.readFromInventoryToCharacter ();
        i.UpdateGuiCounts ();
    }
}
