using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPlayerInventoryContainerInteractionClickHandler: MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {
        transform.root.GetComponentInChildren<ContainerDragHandler> ().CreateSlotWithItem (GetComponent<SlotObjectContainer>().obj);
        GameObject playerReference = transform.root.GetComponent<PlayerReferenceContainer> ().Player;
        DestroyImmediate (gameObject);
        playerReference.GetComponent<UICharacterInventoryFactory> ().RefreshCharacterInventory ();
    }
}