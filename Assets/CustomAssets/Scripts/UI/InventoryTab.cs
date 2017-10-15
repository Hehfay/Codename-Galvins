using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InventoryTab: MonoBehaviour, IPointerClickHandler {

    UIController uiController;

    void Start () {
        uiController = transform.parent.GetComponent<UIController> ();
    }

    public void OnPointerClick (PointerEventData eventData) {
        uiController.InventoryTabClicked ();
    }
}
