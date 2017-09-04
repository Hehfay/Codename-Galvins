using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OkButtonOnClick : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick (PointerEventData eventData) {
        GameObject currentPlayer = GameObject.Find ("Player(Clone)");

        CursorManager cursorManager = currentPlayer.GetComponent<CursorManager>();
        cursorManager.cursorLocked = true;
        cursorManager.listening = true;

        PlayerController2 playerController = currentPlayer.GetComponent<PlayerController2> ();
        playerController.shouldRotate = true;
        playerController.listening = true;

        currentPlayer.GetComponent<Character> ().allowedToPickThingsUp = true;
        currentPlayer.GetComponent<UIManager> ().enabled = true;

        Destroy (GameObject.Find("WhatYouPickedUp(Clone)"));
    }
}
