using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ContainerDragHandler : MonoBehaviour, IDropHandler {
    public GameObject Slot;
    public GameObject ContainerSlotItem;

    public void OnDrop (PointerEventData eventData) {
        if (DragHandler.itemBeingDragged == null) {
            return;
        }
        CreateSlotWithItem (DragHandler.itemBeingDragged.GetComponent<SlotObjectContainer>().obj);

        GameObject containerSlot = null;
        if (DragHandler.itemBeingDragged.transform.parent.GetComponent<ContainerSlot>()) {
            containerSlot = DragHandler.startParent.gameObject;
        }
        if (containerSlot) {
            Destroy (containerSlot);
        }
        DestroyImmediate (DragHandler.itemBeingDragged.transform.gameObject);

        DragHandler.itemBeingDragged = null;
    }

    public void CreateSlotWithItem (GameObject Item) {
        GameObject slot = Instantiate (Slot, transform.GetChild(0).transform.GetChild(0), false);
        GameObject slotItem = Instantiate (ContainerSlotItem, slot.transform, false);
        slotItem.GetComponent<SlotObjectContainer> ().obj = Item;

        GameObject item = slotItem.GetComponent<SlotObjectContainer> ().obj;
        Component objectData = item.GetComponent (typeof(IObjectData));
        IObjectData data = objectData as IObjectData;

        slotItem.GetComponent<Text> ().text = data.objectName ();

        if (data.count () != 1) {
            slotItem.GetComponent<Text> ().text += " x" + data.count ();
        }

        // Process Quest UnTrigger.
        QuestTriggerWrapper questTriggerWrapper = item.GetComponent<QuestTriggerWrapper> ();
        if (questTriggerWrapper != null) {
            QuestTrigger trigger = questTriggerWrapper.questTrigger;
            if (trigger != null) {
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestUnTrigger(trigger);
            }
        }

        // Refresh the player inventory.
        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
    }
}