using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class EquipmentSlot : MonoBehaviour, IDropHandler {

    public EquipSlotType equipmentType;

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

        if (DragHandler.itemBeingDragged == null) {
            return;
        }

        GameObject uiObject = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
        Equipment equipmentComponent = uiObject.GetComponent<Equipment> ();
        if (equipmentComponent == null || 
            !equipmentComponent.equipmentType.Equals(equipmentType)) {
            DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
            return;
        }

        if (DragHandler.startParent.GetComponent<PlayerInventorySlot> () ||
            DragHandler.startParent.GetComponent<EquipmentSlot> ()) {

            if (item) {
                item.transform.SetParent (DragHandler.startParent);
            }
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            DragHandler.itemBeingDragged = null;
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
        }
        else if (DragHandler.startParent.GetComponent<ContainerSlot>()) {
            if (item) {
                GameObject uiObject2 = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
                Equipment equipmentComponent2 = uiObject2.GetComponent<Equipment> ();
                if (equipmentComponent2 == null ||
                    item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment> () == null ||
                    !equipmentComponent2.equipmentType.Equals (equipmentType)) {
                    DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
                    return;
                }

                GameObject itemCopy = item;
                itemCopy.transform.SetParent (DragHandler.startParent);
                DragHandler.itemBeingDragged.transform.SetParent (transform);
                transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();

                CreateSlotItemAsChildOfExistingSlot (DragHandler.itemBeingDragged);
                DragHandler.startParent.GetComponent<ContainerSlot> ().CreateSlotItemAsChildOfExistingSlot (itemCopy);
            }
            else {
                GameObject uiObject2 = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
                Equipment equipmentComponent2 = uiObject2.GetComponent<Equipment> ();
                if (equipmentComponent2 == null ||
                    !equipmentComponent2.equipmentType.Equals (equipmentType)) {
                    DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
                    return;
                }
                DragHandler.itemBeingDragged.transform.SetParent (transform);
                transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();

                CreateSlotItemAsChildOfExistingSlot (DragHandler.itemBeingDragged);
                Destroy (DragHandler.startParent.gameObject);
            }
        }
    }

    public void CreateSlotItemAsChildOfExistingSlot (GameObject item) {
        GameObject newSlotItem = Instantiate (slotItem, transform, false);
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
}