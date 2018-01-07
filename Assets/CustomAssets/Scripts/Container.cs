using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script requires the UIContainerFactory component.
public class Container : MonoBehaviour, IInteractable {
    public string toolTip;
    public List<GameObject> inventory;
    private UIContainerFactory uiContainerFactory;

    public GameObject slotItemPrefab;

    void Start () {
        uiContainerFactory = GetComponent<UIContainerFactory> ();
        uiContainerFactory.enabled = false;
    }

    public void PopulateContainerWithUIData (GameObject UIData) {
        // And also delete empty slots. TODO A different solution.

        inventory.Clear ();

        // Get the "Content"
        GameObject Content = UIData.transform.GetChild (0).transform.GetChild (0).gameObject;

        for (int i = 0; i < Content.transform.childCount; ++i) {
            SlotObjectContainer slotObjectContainer = 
                Content.transform.GetChild(i).GetComponentInChildren<SlotObjectContainer>();

            if (slotObjectContainer != null) {
                inventory.Add (Content.transform.GetChild (i).GetComponentInChildren<SlotObjectContainer> ().obj);
            }
        }
    }

    public bool Interact (GameObject gameobject) {

        InteractUtility.InteractStart (gameobject);

        // Tell the gameobject that interacted with this to draw its inventory
        gameobject.GetComponent<UICharacterInventoryFactory> ().CreateFactoryItem (slotItemPrefab);

        gameobject.GetComponent<PlayerMovementController> ().enabled = false;

        // Disable factory input.
        gameobject.GetComponent<UIInputHandler> ().enabled = false;

        // Create the inventory.
        uiContainerFactory.CreateFactoryItem (inventory, gameobject);
        uiContainerFactory.enabled = true;
        return true;
    }

    public string ToolTip () {
        return toolTip;
    }
}