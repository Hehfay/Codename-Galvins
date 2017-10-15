using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class QuestLogTab : MonoBehaviour, IPointerClickHandler{

    UIController uiController;

    void Start () {
        uiController = transform.parent.GetComponent<UIController> ();
    }

    public void OnPointerClick (PointerEventData eventData) {
        uiController.QuestTabClicked ();
    }
}
