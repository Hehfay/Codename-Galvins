using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContainerSlot : MonoBehaviour, IDropHandler {

    public GameObject slotItem;

    public GameObject item {
        get {
            if (transform.childCount > 0) {
                return transform.GetChild (0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop (PointerEventData eventData) {
        // The only way this function will be called is if the player is trying to swap an item from the inventory
        // with a item on the chest.

        if (DragHandler.startParent.GetComponent<PlayerInventorySlot> ()) {
            // The player is swapping an item from the inventory.

            GameObject itemCopy = item;
            itemCopy.transform.SetParent (DragHandler.startParent);
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();

            itemCopy.transform.SetParent (null);
            DragHandler.itemBeingDragged.transform.SetParent (null);

            CreateSlotItemAsChildOfExistingSlot (DragHandler.itemBeingDragged);
            DragHandler.startParent.GetComponent<PlayerInventorySlot> ().CreateSlotItemAsChildOfExistingSlot (itemCopy);
        }
        else if (DragHandler.startParent.GetComponent<ContainerSlot> ()) {
            item.transform.SetParent (DragHandler.startParent);
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            DragHandler.itemBeingDragged = null;
        }
        else if (DragHandler.startParent.GetComponent<EquipmentSlot>()) {

            GameObject uiObject = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
            Equipment equipmentComponent = uiObject.GetComponent<Equipment> ();
            if (equipmentComponent == null ||
                item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment> () == null ||
                !equipmentComponent.equipmentType.Equals (item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment> ().equipmentType)) {
                DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
                return;
            }

            GameObject itemCopy = item;
            itemCopy.transform.SetParent (DragHandler.startParent);
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();

            CreateSlotItemAsChildOfExistingSlot (DragHandler.itemBeingDragged);
            DragHandler.startParent.GetComponent<EquipmentSlot> ().CreateSlotItemAsChildOfExistingSlot (itemCopy);
        }
    }

    public void CreateSlotItemAsChildOfExistingSlot (GameObject itemcopy) {
        GameObject newSlotItem = Instantiate (slotItem, transform, false);
        Component comp = itemcopy.GetComponent<SlotObjectContainer>().obj.GetComponent (typeof (IObjectData));
        IObjectData objectData = comp as IObjectData;
        string uiText;
        if (objectData.count () == 1) {
            uiText = objectData.objectName ();
        }
        else {
            uiText = objectData.objectName () + " x" + objectData.count ();
        }
        newSlotItem.GetComponent<SlotObjectContainer> ().obj = itemcopy.GetComponent<SlotObjectContainer>().obj;
        newSlotItem.GetComponent<Text> ().text = uiText;

        QuestTriggerWrapper questTriggerWrapper = newSlotItem.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper>();
        if (questTriggerWrapper != null) {
            QuestTrigger trigger = questTriggerWrapper.questTrigger;
            if (trigger != null) {
                Debug.Log ("Process Quest UnTrigger.");
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestUnTrigger(trigger);
            }
        }

        Destroy (itemcopy);
    }
}