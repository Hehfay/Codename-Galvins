using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropButtonOnClick: MonoBehaviour, IPointerClickHandler {

    Text displayCountText;

    void Start () {
        GameObject CountDisplay = transform.parent.gameObject;
        displayCountText = CountDisplay.GetComponentInChildren<Text> ();
        Debug.Assert (displayCountText != null);
    }

    public void OnPointerClick (PointerEventData eventData) {
        UiClickHandler.currentClickHandler.DropStackOfItems(Convert.ToInt32(displayCountText.text));
        CancelButtonOnClick.SelectCountDone ();
    }

}
