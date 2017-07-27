using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

// This script handles the click event for UI elements.
// When a UI element is clicked on, it is dropped.
public class UiClickHandler : MonoBehaviour, IPointerClickHandler {

    public GameObject dropItemPrefab;

    public GameObject UiElementSelectCountPrefab;

    string playerString = "Player(Clone)";


    // TODO Handle dropping a stack of items.
    public void OnPointerClick (PointerEventData eventData) {

        InventoryController i = GameObject.Find ("InventoryController").GetComponent<InventoryController>();

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

        if (itemCount > 1) {
            Debug.Log ("Creating button.");
            GameObject button = Instantiate (UiElementSelectCountPrefab) as GameObject;
            button.transform.SetParent (gameObject.transform.parent.transform.parent.transform.parent.transform);
            button.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
            return;
        }

        if (itemCount > 1) {
            GameObject g = Instantiate (dropItemPrefab) as GameObject;
            g.GetComponent<Pickup>().pickupData = gameObject.GetComponent<Pickup>().pickupData;

            GameObject c = GameObject.Find (playerString);
            g.transform.position = c.transform.position;

            //character.itemCount[index]--;
            gameObject.GetComponent<Pickup> ().count--;
            i.readFromInventoryToCharacter ();
            i.UpdateGuiCounts ();

        }
        else {
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
    }
}
