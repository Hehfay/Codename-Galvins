using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;

// This script handles the click event for UI elements.
// When a UI element is clicked on, it is dropped.
public class UiClickHandler : MonoBehaviour, IPointerClickHandler {
    string playerString = "Player(Clone)";

    public static UiClickHandler currentClickHandler;
    UIController uiController;
    InventoryController inventoryController;
    CharacterInventory character;

    void Start () {
        uiController = GameObject.Find ("Canvas").GetComponent<UIController>();
        inventoryController = GameObject.Find ("Canvas").GetComponent<InventoryController>();
        character = GameObject.Find (playerString).GetComponent<CharacterInventory>();
    }

    public void OnPointerClick (PointerEventData eventData) {

        int index = gameObject.GetComponent<Pickup> ().inventoryIndex;

        int itemCount;

        Transform t = gameObject.transform.parent.transform.parent;

        if (t == inventoryController.rh.transform) {
            itemCount = character.rightHandItemCount[index];
        }
        else if (t == inventoryController.lh.transform) {
            itemCount = character.leftHandItemCount[index];
        }
        else {
            itemCount = character.itemCount[index];
        }

        if (uiController.selectCountEnabled) {
            return;
        }

        if (itemCount > 1) {
            uiController.CreateSelectCountButton ();

            uiController.SelectCountButtonSetText ("1");

            uiController.SelectCountButtonSetMaxCount (GetComponent<Pickup>().count);

            currentClickHandler = this;
            uiController.selectCountEnabled = true;
            inventoryController.DisableDragHandlers ();
            return;
        }

        GameObject dropItem = Resources.Load<GameObject>("Items/" + GetComponent<DataSheetWrapper>().dataSheet.name);

        GameObject c = GameObject.Find (playerString);
        dropItem.transform.position = c.transform.position;

        dropItem.GetComponent<Pickup> ().count = GetComponent<Pickup> ().count;

        Instantiate (dropItem);

        gameObject.transform.SetParent (null);
        Destroy (gameObject);
        // character.itemCount[index]--;
        inventoryController.readFromInventoryToCharacter ();
        inventoryController.UpdateGuiCounts ();

        // TODO Wrap this nicely so other drop logic can use it.

        // See if the item has a quest trigger.
        QuestTrigger pickupQuestTrigger = dropItem.GetComponent<QuestTrigger>();
        if (pickupQuestTrigger != null) {
            GetComponent<QuestManager> ().ProcessQuestUnTrigger(pickupQuestTrigger);
        }
    }

    public void DropStackOfItems (int dropCount) {
        GameObject dropItem = Resources.Load<GameObject>("Items/" + GetComponent<DataSheetWrapper>().dataSheet.name);

        GameObject c = GameObject.Find (playerString);
        dropItem.transform.position = c.transform.position;

        gameObject.GetComponent<Pickup> ().count -= dropCount;

        dropItem.GetComponent<Pickup> ().count = dropCount;

        Instantiate (dropItem);

        if (gameObject.GetComponent<Pickup>().count == 0) {
            gameObject.transform.SetParent (null);
            Destroy (gameObject);
        }

        inventoryController.readFromInventoryToCharacter ();
        inventoryController.UpdateGuiCounts ();
    }
}
