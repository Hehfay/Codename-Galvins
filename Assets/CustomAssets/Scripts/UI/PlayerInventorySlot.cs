using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

// This class is put on the slot object and handles
// moving UI objects around the canvas.
public class PlayerInventorySlot : MonoBehaviour, IDropHandler {

    public GameObject slotItem;
    public GameObject slotItemInteractingWithContainer;


    public GameObject item {
        get {
            if (transform.childCount > 0) {
                return transform.GetChild (0).gameObject;
            }
            return null;
        }
    }

    public void CreateSlotItemAsChildOfExistingSlot (GameObject item) {
        GameObject newSlotItem = Instantiate (slotItemInteractingWithContainer, transform, false);
        Component comp = item.GetComponent<SlotObjectContainer>().obj.GetComponent (typeof (IObjectData));
        IObjectData objectData = comp as IObjectData;
        string uiText;
        if (objectData.count () == 1) {
            uiText = objectData.objectName ();
        }
        else {
            uiText = objectData.objectName () + " x" + objectData.count ();
        }
        newSlotItem.GetComponent<SlotObjectContainer> ().obj = item.GetComponent<SlotObjectContainer>().obj;
        newSlotItem.GetComponent<Text> ().text = uiText;

        QuestTriggerWrapper questTriggerWrapper = newSlotItem.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper>();
        if (questTriggerWrapper != null) {
            QuestTrigger trigger = questTriggerWrapper.questTrigger;
            if (trigger != null) {
                Debug.Log ("Process Quest Trigger.");
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger(trigger);
            }
        }

        Destroy (item);
    }

    public void OnDrop (PointerEventData eventData) {

        if (DragHandler.itemBeingDragged == null) {
            return;
        }

        if (DragHandler.startParent.GetComponent<PlayerInventorySlot> ()) {
            // This is the easiest case. The Player is swapping items around in the inventory.
            // No need to process a quest trigger or anything like that.
            if (item) {
                item.transform.SetParent (DragHandler.startParent);
            }
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            DragHandler.itemBeingDragged = null;
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
        }
        else if (DragHandler.startParent.GetComponent<EquipmentSlot> ()) {
            // The Player is trying to drag an item from an equipment slot to an inventory slot.
            // If there is no item in this slot, no big deal. Else they are trying to swap and we
            // need to be a little careful.

            if (item) {
                GameObject uiObject = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
                Equipment equipmentComponent = uiObject.GetComponent<Equipment> ();
                if (equipmentComponent == null ||
                    item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment> () == null ||
                    !equipmentComponent.equipmentType.Equals (item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment> ().equipmentType)) {
                    DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
                    return;
                }
                item.transform.SetParent (DragHandler.startParent);
            }
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            DragHandler.itemBeingDragged = null;
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
        }
        else if (DragHandler.startParent.GetComponent<ContainerSlot>()) {
            // The Player is trying to move an item from a container to an inventory slot.
            // It may be a swap or it may not be.
            if (item) {
                GameObject itemCopy = item;
                itemCopy.transform.SetParent (null);

                DragHandler.itemBeingDragged.transform.SetParent (transform);
                transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
                DragHandler.itemBeingDragged.transform.SetParent (null);

                // Create a slot item.
                GameObject newSlotItem = Instantiate (slotItemInteractingWithContainer, transform, false);
                Component comp = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer>().obj.GetComponent (typeof (IObjectData));
                IObjectData objectData = comp as IObjectData;

                string uiText;
                if (objectData.count () == 1) {
                    uiText = objectData.objectName ();
                }
                else {
                    uiText = objectData.objectName () + " x" + objectData.count ();
                }
                newSlotItem.GetComponent<Text> ().text = uiText;
                newSlotItem.GetComponent<SlotObjectContainer> ().obj = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer>().obj;

                // Tell the container to create its slot item.
                Destroy (DragHandler.itemBeingDragged);
                DragHandler.startParent.GetComponent<ContainerSlot> ().CreateSlotItemAsChildOfExistingSlot (itemCopy);

                QuestTriggerWrapper questTriggerWrapper = newSlotItem.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper>();
                if (questTriggerWrapper != null) {
                    QuestTrigger trigger = questTriggerWrapper.questTrigger;
                    if (trigger != null) {
                        Debug.Log ("Process Quest Trigger.");
                        GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger(trigger);
                    }
                }
            }
            else {
                // Not a swap.
                GameObject itemCopy = item;
                Destroy (item);
                DragHandler.itemBeingDragged.transform.SetParent (transform);
                transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();

                // Create a slot item.
                itemCopy = item;
                Destroy (item);

                GameObject newSlotItem = Instantiate (slotItemInteractingWithContainer, transform, false);
                Component comp = itemCopy.GetComponent<SlotObjectContainer>().obj.GetComponent (typeof (IObjectData));
                IObjectData objectData = comp as IObjectData;

                string uiText;
                if (objectData.count () == 1) {
                    uiText = objectData.objectName ();
                }
                else {
                    uiText = objectData.objectName () + " x" + objectData.count ();
                }
                newSlotItem.GetComponent<Text> ().text = uiText;
                newSlotItem.GetComponent<SlotObjectContainer> ().obj = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer>().obj;

                QuestTriggerWrapper questTriggerWrapper = newSlotItem.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper>();
                if (questTriggerWrapper != null) {
                    QuestTrigger trigger = questTriggerWrapper.questTrigger;
                    if (trigger != null) {
                        Debug.Log ("Process Quest Trigger.");
                        GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger(trigger);
                    }
                }

                Destroy (DragHandler.startParent.gameObject);
            }
        }
    }
}