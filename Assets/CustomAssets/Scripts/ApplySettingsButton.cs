using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ApplySettingsButton: MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick (PointerEventData eventData) {

        bool rememberUIState = transform.parent.GetChild (0).GetComponent<Toggle>().isOn;
        string framerateAsString = (transform.parent.GetChild (1).GetComponent<InputField> ().text);

        int framerate = -1;
        if (framerateAsString != "") {
            framerate = Convert.ToInt32 (framerateAsString);
        }
        else {
            // Keep the framerate as whatever it was.
            framerate = transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<PlayerSettings> ().framerate;
        }

        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<PlayerSettings> ().ApplySettings (rememberUIState, framerate);
    }
}