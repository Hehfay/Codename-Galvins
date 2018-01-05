﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OptionsTab: MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {

        if (UIState.uiState == UIState.UIStateEnum.Options) {
            return;
        }

        switch (UIState.uiState) {
            case UIState.UIStateEnum.Inventory:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().DestroyFactoryItem ();
            break;

            case UIState.UIStateEnum.QuestLog:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIQuestLogFactory> ().DestroyFactoryItem ();
            break;
        }

        UIState.uiState = UIState.UIStateEnum.Options;
        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIOptionsFactory> ().CreateFactoryItem ();
    }
}