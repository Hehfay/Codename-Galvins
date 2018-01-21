using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerSettings : MonoBehaviour {
    public bool rememberUIState;
    public int framerate;

    void Start () {
        if (GetComponent<NetworkIdentity>().isLocalPlayer) {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = framerate;
        }
    }

    public void ApplySettings (bool rememberUIStateArgument, int framerateArgument) {
        rememberUIState = rememberUIStateArgument;
        framerate = framerateArgument;
        Application.targetFrameRate = framerate;
    }
}