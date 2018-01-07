using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OkButtonOnClick : MonoBehaviour, IPointerClickHandler {

    GameObject currentPlayer;

    void Start () {
    }

    public void OnPointerClick (PointerEventData eventData) {
        currentPlayer = GameObject.Find ("A03(Clone)");
        CursorManager cursorManager = currentPlayer.GetComponent<CursorManager>();
        cursorManager.cursorLocked = true;

        PlayerMovementController playerController = currentPlayer.GetComponent<PlayerMovementController> ();
        playerController.shouldRotate = true;

        //currentPlayer.GetComponent<ColliderInteractController> ().allowedToPickThingsUp = true;
        // uiController.DestroyWhatWasPickedUp ();
    }
}