﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class allows the player to pick things up
// and manages what you have equipped.
public class CharacterInventory : MonoBehaviour {

    int INVENTORY_SIZE = 14;

    // You can hold 3 weapons in each hand.
    // The third index of your hand slots is always
    // the bare hand.
    int NUM_SLOTS_PER_HAND = 4;

    public GameObject[] leftHand;
    public GameObject[] rightHand;

    // Not implemented yet.
    public Pickup[] armor;

    // When you pickup an item, its data is stored
    // in this array.
    public DataSheet[] loot;

    // TODO REDO
    public int[] itemCount;
    public int[] leftHandItemCount;
    public int[] rightHandItemCount;


    // Use this for initialization
    void Start () {
        // You should always have your left hand and right hand in the array.

        if (leftHand[3] == null) {
            leftHand[3] = Resources.Load<GameObject> ("Items/LeftHand");
        }

        if (rightHand[3] == null) {
            rightHand[3] = Resources.Load<GameObject> ("Items/RightHand");
        }

        // For the items in your hands, set the transform parent.
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                if (leftHand[i].GetComponent<MeshRenderer> () != null) {
                    leftHand[i].GetComponent<MeshRenderer> ().enabled = false;
                    leftHand[i].gameObject.transform.SetParent (transform.parent);
                }
            }
            if (rightHand[i] != null) {
                if (rightHand[i].GetComponent<MeshRenderer> () != null) {
                    rightHand[i].GetComponent<MeshRenderer> ().enabled = false;
                    rightHand[i].gameObject.transform.SetParent (transform.parent);
                }
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                leftHand[i].GetComponent<Pickup>().active = true;
                break;
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].GetComponent<Pickup>().active = true;
                break;
            }
        }

        // Update the hand counts here so we don't have to in the UI.
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHandItemCount[i]++;
            }
            if (leftHand[i] != null) {
                leftHandItemCount[i]++;
            }
        }
        // updateGuiText ();

        RegisterCharacterInventoryToInventoryController();
	}

    public void PickupLogic (Collider copy) {

        string whatWasPickedUp = "";

        DataSheetWrapper[] dataSheetWrapper = copy.gameObject.GetComponents<DataSheetWrapper> ();
        Pickup[] pickup = copy.gameObject.GetComponents<Pickup> ();
        if (dataSheetWrapper.Length == 0) {
            return;
        }

        int numPickedUp = 0;
        for (int i = 0; i < dataSheetWrapper.Length; ++i) {
            bool foundSlotForItem = false;
            if (dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.stackable) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet == loot[j]) {
                        itemCount[j] += pickup[i].count;
                        foundSlotForItem = true;
                        whatWasPickedUp += dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.equipmentName + " x" + pickup[i].count.ToString () + "\n";
                        Destroy (dataSheetWrapper[i]);
                        numPickedUp++;
                    }
                }
            }
            if (!foundSlotForItem) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (loot[j] == null) {
                        itemCount[j] += pickup[i].count;
                        loot[j] = dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet;
                        whatWasPickedUp += dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.equipmentName + " x" + pickup[i].count.ToString () + "\n";
                        Destroy (dataSheetWrapper[i]);
                        numPickedUp++;
                        break;
                    }
                }
            }
        }
        if (numPickedUp == dataSheetWrapper.Length) {
            Destroy (copy.gameObject);
        }
        GetComponent<ColliderInteractController> ().DisplayWhatWasPickedUp (whatWasPickedUp);
    }

    private void RegisterCharacterInventoryToInventoryController () {
        GameObject.Find ("Canvas").GetComponent<InventoryController> ().RegisterCharacterInventoryToInventoryController (this);
    }
}