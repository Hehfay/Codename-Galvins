using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

// This class is put on the slot object and handles
// moving UI objects around the canvas.
public class Slot : MonoBehaviour, IDropHandler, IPointerClickHandler {

    bool canvasEnabled;

    public void Start () {
        canvasEnabled = true;
    }

    public GameObject item {
        get {
            if (transform.childCount > 0) {
                return transform.GetChild (0).gameObject;
            }
            return null;
        }
    }

    public void OnDrop (PointerEventData eventData) {

        GameObject leftHand = GameObject.Find ("LeftHand");
        GameObject rightHand = GameObject.Find ("RightHand");

        Debug.Assert (leftHand);
        Debug.Assert (rightHand);

        if (!item) {
            // This handles the case of trying to put a stackable item in the
            // left or right hand.
            if ((transform.parent == leftHand.transform ||
                transform.parent == rightHand.transform) &&
                DragHandler.itemBeingDragged.GetComponent<Pickup>().pickupData.stackable) {

                DragHandler.itemBeingDragged.transform.SetParent (DragHandler.startParent);
                return;
            }

            DragHandler.itemBeingDragged.transform.SetParent (transform);
        }
        else {
            // DragHandler.itemBeingDragged is stackable and being swapped with an item in left or right hand.
            if ((item.transform.parent.parent.transform == leftHand.transform ||
                item.transform.parent.parent.transform == rightHand.transform) &&
                DragHandler.itemBeingDragged.GetComponent<Pickup>().pickupData.stackable) {
                return;
            }

            if (DragHandler.itemBeingDragged.transform.parent.parent.transform == leftHand.transform ||
                DragHandler.itemBeingDragged.transform.parent.parent.transform == rightHand.transform &&
                item.GetComponent<Pickup>().pickupData.stackable) {
                return;
            }
            DragHandler.itemBeingDragged.transform.SetParent (transform);
            item.transform.SetParent (DragHandler.startParent);
        }
    }

    public void OnPointerClick (PointerEventData eventData) {
        canvasEnabled = false;
    }
}