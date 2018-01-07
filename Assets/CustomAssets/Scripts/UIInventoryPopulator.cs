using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryPopulator : MonoBehaviour {

    private GameObject _slotItemPrefab;

    public void DisplayCharacterInventory (CharacterInventory characterInventory, GameObject slotItemPrefab) {

        _slotItemPrefab = slotItemPrefab;

        GameObject RightHand = transform.GetChild (0).gameObject;
        GameObject LeftHand = transform.GetChild (1).gameObject;
        GameObject Equipment = transform.GetChild (2).gameObject;
        GameObject Inventory = transform.GetChild (3).gameObject;

        Debug.Assert (RightHand != null);
        Debug.Assert (LeftHand != null);
        Debug.Assert (Equipment != null);
        Debug.Assert (Inventory != null);

        GameObject Head = Equipment.transform.GetChild (0).gameObject;
        GameObject Chest = Equipment.transform.GetChild (1).gameObject;
        GameObject Hands = Equipment.transform.GetChild (2).gameObject;
        GameObject Feet = Equipment.transform.GetChild (3).gameObject;

        Debug.Assert (Head != null);
        Debug.Assert (Chest != null);
        Debug.Assert (Hands != null);
        Debug.Assert (Feet != null);

        // Destroy all the children so this function can be used to refresh the inventory.
        // Left hand right hand.
        for (int i = 0; i < RightHand.transform.childCount; ++i) {
            if (RightHand.transform.GetChild(i).childCount != 0) {
                Destroy (RightHand.transform.GetChild(i).GetChild(0).gameObject);
            }
            if (LeftHand.transform.GetChild(i).childCount != 0) {
                Destroy (LeftHand.transform.GetChild(i).GetChild(0).gameObject);
            }
        }
        // Head Chest Hands Feet
        if (Head.transform.GetChild(0).transform.childCount != 0) {
            Destroy (Head.transform.GetChild(0).transform.GetChild(0).gameObject);
        }
        if (Chest.transform.GetChild(0).transform.childCount != 0) {
            Destroy (Chest.transform.GetChild(0).transform.GetChild(0).gameObject);
        }
        if (Hands.transform.GetChild(0).transform.childCount != 0) {
            Destroy (Hands.transform.GetChild(0).transform.GetChild(0).gameObject);
        }
        if (Feet.transform.GetChild(0).transform.childCount != 0) {
            Destroy (Feet.transform.GetChild(0).transform.GetChild(0).gameObject);
        }
        // Inventory
        for (int i = 0; i < Inventory.transform.childCount; ++i) {
            if (Inventory.transform.GetChild(i).childCount != 0) {
                Destroy (Inventory.transform.GetChild(i).GetChild(0).gameObject);
            }
        }

        // Populate the UI.
        // Left and Right Hands.
        for (int i = 0; i < characterInventory.leftHand.Length - 1; ++i) {
            if (characterInventory.rightHand[i] != null) {
                GameObject newSlotItem = Instantiate (_slotItemPrefab, RightHand.transform.GetChild (i), false);
                newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.rightHand[i];
                Component objectDataComponent = characterInventory.rightHand[i].GetComponent (typeof (IObjectData));
                IObjectData objectData = objectDataComponent as IObjectData;
                newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
                if (objectData.count () != 1) {
                    newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
                }
            }
            if (characterInventory.leftHand[i] != null) {
                GameObject newSlotItem = Instantiate (_slotItemPrefab, LeftHand.transform.GetChild (i), false);
                newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.leftHand[i];
                Component objectDataComponent = characterInventory.leftHand[i].GetComponent (typeof (IObjectData));
                IObjectData objectData = objectDataComponent as IObjectData;
                newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
                if (objectData.count () != 1) {
                    newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
                }
            }
        }

        // Head
        if (characterInventory.head != null) {
            GameObject newSlotItem = Instantiate (_slotItemPrefab, Head.transform.GetChild(0), false);
            newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.head;
            Component objectDataComponent = characterInventory.head.GetComponent (typeof (IObjectData));
            IObjectData objectData = objectDataComponent as IObjectData;
            newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
            if (objectData.count () != 1) {
                newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
            }
        }

        // Chest
        if (characterInventory.chest != null) {
            GameObject newSlotItem = Instantiate (_slotItemPrefab, Chest.transform.GetChild(0), false);
            newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.chest;
            Component objectDataComponent = characterInventory.chest.GetComponent (typeof (IObjectData));
            IObjectData objectData = objectDataComponent as IObjectData;
            newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
            if (objectData.count () != 1) {
                newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
            }
        }

        // Hands
        if (characterInventory.hands != null) {
            GameObject newSlotItem = Instantiate (_slotItemPrefab, Hands.transform.GetChild(0), false);
            newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.hands;
            Component objectDataComponent = characterInventory.hands.GetComponent (typeof (IObjectData));
            IObjectData objectData = objectDataComponent as IObjectData;
            newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
            if (objectData.count () != 1) {
                newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
            }
        }

        // Feet
        if (characterInventory.feet != null) {
            GameObject newSlotItem = Instantiate (_slotItemPrefab, Feet.transform.GetChild(0), false);
            newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.feet;
            Component objectDataComponent = characterInventory.feet.GetComponent (typeof (IObjectData));
            IObjectData objectData = objectDataComponent as IObjectData;
            newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
            if (objectData.count () != 1) {
                newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
            }
        }

        // Inventory
        Debug.Assert (characterInventory.inventory.Count == 5);
        for (int i = 0; i < characterInventory.inventory.Count; ++i) {
            if (characterInventory.inventory[i] != null) {
                GameObject newSlotItem = Instantiate (_slotItemPrefab, Inventory.transform.GetChild (i), false);
                newSlotItem.GetComponent<SlotObjectContainer> ().obj = characterInventory.inventory[i];
                Component objectDataComponent = characterInventory.inventory[i].GetComponent (typeof (IObjectData));
                IObjectData objectData = objectDataComponent as IObjectData;
                newSlotItem.GetComponent<Text> ().text = objectData.objectName ();
                if (objectData.count () != 1) {
                    newSlotItem.GetComponent<Text> ().text += " x" + objectData.count ();
                }
            }
        }
    }

    public void PopulateCharacterInventory (ref CharacterInventory characterInventory) {
        GameObject RightHand = transform.GetChild (0).gameObject;
        GameObject LeftHand = transform.GetChild (1).gameObject;
        GameObject Equipment = transform.GetChild (2).gameObject;
        GameObject Inventory = transform.GetChild (3).gameObject;

        Debug.Assert (RightHand != null);
        Debug.Assert (LeftHand != null);
        Debug.Assert (Equipment != null);
        Debug.Assert (Inventory != null);

        GameObject Head = Equipment.transform.GetChild (0).gameObject;
        GameObject Chest = Equipment.transform.GetChild (1).gameObject;
        GameObject Hands = Equipment.transform.GetChild (2).gameObject;
        GameObject Feet = Equipment.transform.GetChild (3).gameObject;

        Debug.Assert (Head != null);
        Debug.Assert (Chest != null);
        Debug.Assert (Hands != null);
        Debug.Assert (Feet != null);

        // Zero out the characterInventory.
        for (int i = 0; i < characterInventory.leftHand.Length - 1; ++i) {
            characterInventory.leftHand[i] = null;
            characterInventory.rightHand[i] = null;
            if (LeftHand.transform.GetChild(i).childCount != 0) {
                characterInventory.leftHand[i] = LeftHand.transform.GetChild (i).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
            }
            if (RightHand.transform.GetChild(i).childCount != 0) {
                characterInventory.rightHand[i] = RightHand.transform.GetChild (i).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
            }
        }
        for (int i = 0; i < characterInventory.inventory.Count; ++i) {
            characterInventory.inventory[i] = null;
            if (Inventory.transform.GetChild (i).childCount != 0) {
                characterInventory.inventory[i] = Inventory.transform.GetChild (i).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
            }
        }

        characterInventory.head = null;
        characterInventory.chest = null;
        characterInventory.hands = null;
        characterInventory.feet = null;

        if (Head.transform.GetChild(0).childCount != 0) {
            characterInventory.head = Head.transform.GetChild (0).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
        }
        if (Chest.transform.GetChild(0).childCount != 0) {
            characterInventory.chest = Chest.transform.GetChild (0).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
        }
        if (Hands.transform.GetChild(0).childCount != 0) {
            characterInventory.hands = Hands.transform.GetChild (0).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
        }
        if (Feet.transform.GetChild(0).childCount != 0) {
            characterInventory.feet = Feet.transform.GetChild (0).GetChild (0).gameObject.GetComponent<SlotObjectContainer> ().obj;
        }
    }
}