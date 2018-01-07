using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// In the future this class could be used to the save the UI state when opening
// and closing.
public class UIState : MonoBehaviour {

    public enum UIStateEnum {
        Inventory,
        QuestLog,
        Options,
    };

    public static UIStateEnum uiState;

    void Start () {
        uiState = UIStateEnum.Inventory;
    }
}