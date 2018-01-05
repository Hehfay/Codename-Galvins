using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GoodbyeButtonOnClick : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {

        //transform.parent.GetComponent<UIController> ().DisableDialog ();
        //transform.parent.GetComponent<UIController> ().DestroyTopics ();

        // TODO TRY TO REDO THIS. NEED TO GET AWAY FROM GameObject.Find
        //GameObject.Find("A03(Clone)").GetComponent<ColliderInteractController> ().allowedToPickThingsUp = true;
    }
}
