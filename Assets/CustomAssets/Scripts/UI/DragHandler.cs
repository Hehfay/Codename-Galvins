using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

// This script is attached to the gameobject that is a child of the slot object.
// This script handles the drag lifecycle of the gameobject being moved.
// This script calls readFromInventoryToCharacter on the InventoryController script
// to update the character data with the latest Ui information.
public class DragHandler: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

    InventoryController invCont;

    void Start () {
        invCont = transform.parent.transform.parent.transform.parent.GetComponent<InventoryController>();
    }

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
    public static Transform startParent;

    public void OnBeginDrag (PointerEventData eventData) {
        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup> ().blocksRaycasts = false;
    }

    public void OnDrag (PointerEventData eventData) {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag (PointerEventData eventData) {
        itemBeingDragged = null;
        GetComponent<CanvasGroup> ().blocksRaycasts = true;
        if (transform.parent == startParent) {
            transform.position = startPosition;
        }
        else {
            invCont.readFromInventoryToCharacter ();
        }
    }
}