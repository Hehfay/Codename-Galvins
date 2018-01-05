using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupObject : MonoBehaviour, IInteractable {

    public string ToolTip () {
        Component component = GetComponent (typeof(IObjectData));
        IObjectData objData = component as IObjectData;
        if (objData.count() != 1) {
            return objData.toolTip () + " x" + objData.count ();
        }
        return objData.toolTip ();
    }

    public bool Interact (GameObject gameobject) {
        CharacterInventory characterInventory = gameobject.GetComponent<CharacterInventory> ();
        Debug.Assert (characterInventory != null);
        bool foundSlotForItem = false;

        Component component = GetComponent (typeof(IObjectData));
        IObjectData objData = component as IObjectData;

        if (objData.stackable()) {
            for (int i = 0; i < characterInventory.INVENTORY_SIZE; ++i) {
                if (characterInventory.inventory[i] != null) {
                    Component comp = characterInventory.inventory[i].GetComponent (typeof (IObjectData));
                    IObjectData objectData = comp as IObjectData;

                    // This equality check is hacky. It incorrectly assumes every
                    // object in the game will have a unique name.
                    if (objectData.objectName () == objData.objectName ()) {
                        objectData.increaseCount (objData.count ());
                        foundSlotForItem = true;
                        // We added this item to a stack.
                        Destroy (gameObject);
                        break;
                    }
                }
            }
        }

        if (!foundSlotForItem) {
            for (int i = 0; i < characterInventory.INVENTORY_SIZE; ++i) {
                if (characterInventory.inventory[i] == null) {
                    // Assign the inventory slot to this gameObject (not the one that was passed in).
                    characterInventory.inventory[i] = gameObject;
                    foundSlotForItem = true;
                    break;
                }
            }
        }

        if (foundSlotForItem) {
            gameObject.transform.position = gameobject.transform.position;
            QuestTriggerWrapper qt = GetComponent<QuestTriggerWrapper> ();
            if (qt != null) {
                GameObject.Find ("QuestManager").GetComponent<QuestManager>().ProcessQuestTrigger(qt.questTrigger);
            }
            if (gameObject.activeInHierarchy) {
                gameObject.SetActive (false);
            }
        }
        return foundSlotForItem;
    }
}