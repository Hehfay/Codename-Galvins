using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour {
    public bool rememberUIState;
    public int framerate;

    void Start () {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = framerate;
    }

    public void ApplySettings (bool rememberUIStateArgument, int framerateArgument) {
        rememberUIState = rememberUIStateArgument;
        framerate = framerateArgument;
        Application.targetFrameRate = framerate;
    }
}