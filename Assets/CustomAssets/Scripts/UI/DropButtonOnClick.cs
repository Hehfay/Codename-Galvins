using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropButtonOnClick: MonoBehaviour, IPointerClickHandler {

    Text displayCountText;
    UIController uiController;

    void Start () {
        GameObject CountDisplay = transform.parent.gameObject;
        displayCountText = CountDisplay.GetComponentInChildren<Text> ();
        Debug.Assert (displayCountText != null);
        uiController = transform.parent.transform.parent.GetComponent<UIController>();
    }

    public void OnPointerClick (PointerEventData eventData) {
        UiClickHandler.currentClickHandler.DropStackOfItems(Convert.ToInt32(displayCountText.text));
        transform.parent.transform.parent.GetComponent<InventoryController> ().EnableDragHandlers ();
        uiController.DestoryCountButton ();
    }
}
