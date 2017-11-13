using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodbyeButtonOnClick : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {
        transform.parent.GetComponent<UIController> ().DisableDialog ();
        transform.parent.GetComponent<UIController> ().DestroyTopics ();
    }
}
