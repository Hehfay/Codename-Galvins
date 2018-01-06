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

        DragHandler.itemBeingDragged.transform.position = transform.position;

        GameObject uiObject = DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer> ().obj;

        Equipment equipmentComponent = uiObject.GetComponent<Equipment> ();

        if (equipmentComponent == null || 
            !equipmentComponent.equipmentType.Equals(equipmentType)) {
            DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
            return;
        }
        if (item) {
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