using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour, IInteractable {
    // If the equipment is not active, it means the character
    // is not currently using this combat equipment.
    public bool active;
    public int inventoryIndex;
    public int count;

    public bool stackable;

    public string ToolTip () {
        if (count != 1) {
            return GetComponent<DataSheetWrapper> ().dataSheet.equipmentName.ToString () + " x" + count.ToString();
        }
        return GetComponent<DataSheetWrapper> ().dataSheet.equipmentName.ToString ();
    }

    public bool Interact (GameObject gameobject) {
        Inventory inventory = gameobject.GetComponent<Inventory> ();
        Debug.Assert (inventory != null);
        bool foundSlotForItem = false;

        string whatWasPickedUp = "Inventory Full";

        if (stackable) {
            for (int i = 0; i < inventory.INVENTORY_SIZE; ++i) {
                if (inventory.loot[i] == GetComponent<DataSheetWrapper>().dataSheet) {
                    inventory.itemCount[i] += count;

                    whatWasPickedUp = GetComponent<DataSheetWrapper> ().dataSheet.equipmentName;
                    whatWasPickedUp += " x" + count.ToString ();

                    foundSlotForItem = true;
                    break;
                }
            }
        }

        if (!foundSlotForItem) {
            for (int i = 0; i < inventory.INVENTORY_SIZE; ++i) {
                if (inventory.loot[i] == null) {
                    inventory.loot[i] = GetComponent<DataSheetWrapper> ().dataSheet;
                    inventory.itemCount[i] += count;

                    whatWasPickedUp = GetComponent<DataSheetWrapper> ().dataSheet.equipmentName;
                    whatWasPickedUp += " x" + count.ToString ();

                    foundSlotForItem = true;
                    break;
                }
            }
        }

        gameobject.GetComponent<ColliderInteractController> ().DisplayWhatWasPickedUp (whatWasPickedUp);

        if (foundSlotForItem) {

            QuestTriggerWrapper qt = GetComponent<QuestTriggerWrapper> ();

            if (qt != null) {

                GameObject.Find ("QuestManager").GetComponent<QuestManager>().ProcessQuestTrigger(qt.questTrigger);
                // gameobject.GetComponent<QuestManager> ().ProcessQuestTrigger (qt.questTrigger);
            }
            gameobject.GetComponent<ColliderInteractController> ().DestroyWhatWasPickedUp ();

            if (gameObject.activeInHierarchy) {
                Destroy (gameObject);
            }
        }
        return foundSlotForItem;
    }
}