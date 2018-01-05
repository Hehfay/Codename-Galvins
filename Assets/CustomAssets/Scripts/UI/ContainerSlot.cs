using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class ContainerSlot : MonoBehaviour, IDropHandler {

    public GameObject item {
        get {
            if (transform.childCount > 0) {
                return transform.GetChild (0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop (PointerEventData eventData) {
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
        DragHandler.itemBeingDragged.transform.SetParent (transform);

        QuestTriggerWrapper questTriggerWrapper = item.GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper> ();
        if (questTriggerWrapper != null) {
            QuestTrigger trigger = questTriggerWrapper.questTrigger;
            if (trigger != null) {
                Debug.Log ("Process Quest UnTrigger");
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestUnTrigger(trigger);
            }
        }
    }
}