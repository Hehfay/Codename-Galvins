using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OkButtonOnClick : MonoBehaviour, IPointerClickHandler {

    GameObject currentPlayer;
    UIController uiController;

    void Start () {
        uiController = GameObject.Find ("Canvas").GetComponent<UIController>();
    }

    public void OnPointerClick (PointerEventData eventData) {
        currentPlayer = GameObject.Find ("Player(Clone)");

        CursorManager cursorManager = currentPlayer.GetComponent<CursorManager>();
        cursorManager.cursorLocked = true;
        cursorManager.listening = true;

        PlayerMovementController playerController = currentPlayer.GetComponent<PlayerMovementController> ();
        playerController.shouldRotate = true;
        playerController.listening = true;

        currentPlayer.GetComponent<ColliderInteractController> ().allowedToPickThingsUp = true;
        uiController.DestoryWhatWasPickedUp ();
    }
}