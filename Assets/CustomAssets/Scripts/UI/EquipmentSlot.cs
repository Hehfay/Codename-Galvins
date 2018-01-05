using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class EquipmentSlot : MonoBehaviour, IDropHandler {

    public EquipSlotType equipmentType;

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
        DragHandler.itemBeingDragged.transform.SetParent (transform);

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