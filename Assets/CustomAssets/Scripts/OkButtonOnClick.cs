using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OkButtonOnClick : MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick (PointerEventData eventData) {
        // TODO Cleanup.
        GameObject.Find ("Player(Clone)").GetComponent<CursorManager> ().cursorLocked = true;
        GameObject.Find ("Player(Clone)").GetComponent<CursorManager> ().listening = true;
        GameObject.Find ("Player(Clone)").GetComponent<PlayerController> ().shouldRotate = true;
        GameObject.Find ("Player(Clone)").GetComponent<PlayerController> ().listening = true;
        GameObject.Find ("Player(Clone)").GetComponent<Character> ().allowedToPickThingsUp = true;
        GameObject.Find ("Player(Clone)").GetComponent<UIManager> ().enabled = true;
        Destroy (GameObject.Find("WhatYouPickedUp(Clone)"));
    }
}
