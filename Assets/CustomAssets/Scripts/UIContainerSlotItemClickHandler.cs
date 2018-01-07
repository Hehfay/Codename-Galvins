using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIContainerSlotItemClickHandler: MonoBehaviour, IPointerClickHandler {

    public GameObject slotItemPrefab;

    public void OnPointerClick (PointerEventData eventData) {
        GameObject Player = transform.root.GetComponent<PlayerReferenceContainer> ().Player;
        GameObject SlotObject = GetComponent<SlotObjectContainer> ().obj;

        Component interactComponentType = SlotObject.GetComponent (typeof(IInteractable));
        IInteractable interact = interactComponentType as IInteractable;

        if (interact.Interact (Player)) {
            Player.GetComponent<UICharacterInventoryFactory> ().RefreshInventoryUI (slotItemPrefab);
            GameObject parent = gameObject.transform.parent.gameObject;
            Destroy (gameObject);
            Destroy (parent);
        }
    }
}