using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class allows the player to pick things up
// and manages what you have equipped.
public class Inventory : MonoBehaviour {

    public int INVENTORY_SIZE = 14;

    // You can hold 3 weapons in each hand.
    // The third index of your hand slots is always
    // the bare hand.
    int NUM_SLOTS_PER_HAND = 4;

    // TODO remove the left and right hand for a different class.
    public GameObject[] leftHand;
    public GameObject[] rightHand;

    // Not implemented yet.
    public PickupItem[] armor;

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
                leftHand[i].GetComponent<PickupItem>().active = true;
                break;
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].GetComponent<PickupItem>().active = true;
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
        Debug.Log ("NOT IN USE ANYMORE!");
    }

    private void RegisterCharacterInventoryToInventoryController () {
        GameObject.Find ("Canvas").GetComponent<InventoryController> ().RegisterCharacterInventoryToInventoryController (this);
    }
}