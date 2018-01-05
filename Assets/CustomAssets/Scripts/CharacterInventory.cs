using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class allows the player to pick things up
// and manages what you have equipped.
public class CharacterInventory : Inventory {
    public int INVENTORY_SIZE;

    // You can hold 3 weapons in each hand.
    // The third index of your hand slots is always
    // the bare hand.
    int NUM_SLOTS_PER_HAND = 4;

    public GameObject[] leftHand;
    public GameObject[] rightHand;

    public GameObject chest;
    public GameObject head;
    public GameObject hands;
    public GameObject feet;

    // Use this for initialization
    void Start () {
        // Assert that the left hand and right hand gameobjects are where they
        // should be.
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
	}
}