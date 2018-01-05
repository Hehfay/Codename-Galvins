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

        if (item) {
            GameObject itemBeingDraggedSlot = DragHandler.itemBeingDragged.transform.parent.gameObject;
            EquipmentSlot equipmentSlot = itemBeingDraggedSlot.GetComponent<EquipmentSlot> ();
            if (equipmentSlot != null) {
                // This is a red flag because the item being dragged is coming from an equipment slot.
                // Need to make sure that the item in this slot is of the same type.
                Equipment equipment = item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment>();
                if (equipment == null) {
                    return; // Cannot swap.
                }

                if (!equipmentSlot.equipmentType.Equals(item.GetComponent<SlotObjectContainer> ().obj.GetComponent<Equipment>().equipmentType)) {
                    return; // Cannot swap.
                }
            }
            item.transform.SetParent (DragHandler.startParent);
        }


        GameObject containerSlot = null;
        if (DragHandler.itemBeingDragged.transform.parent.GetComponent<ContainerSlot> ()) {
            containerSlot = DragHandler.startParent.gameObject;

            GameObject objCopy = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;
            GameObject newSlotItem = Instantiate (slotItem, transform, false);
            newSlotItem.GetComponent<SlotObjectContainer> ().obj = objCopy;
            newSlotItem.GetComponentInChildren<Text> ().text = DragHandler.itemBeingDragged.GetComponentInChildren<Text> ().text;
            DragHandler.itemBeingDragged = null;
        }
        else {
            DragHandler.itemBeingDragged.transform.SetParent (transform);
        }

        Destroy (containerSlot);

        QuestTriggerWrapper questTriggerWrapper = item.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper> ();
        if (questTriggerWrapper != null) {
            QuestTrigger trigger = questTriggerWrapper.questTrigger;
            if (trigger != null) {
                Debug.Log ("Process Quest Trigger");
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger(trigger);
            }
        }
        Debug.Log ("refreshing inventory");
        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
    }
}