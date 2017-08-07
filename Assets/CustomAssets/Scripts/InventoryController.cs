using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This script is responsible for communicating UI changes from the UI to the
// character and from the character to the UI.
public class InventoryController: MonoBehaviour {

    public GameObject slotItemPrefab;

    public Character character;

    public GameObject lh;
    public GameObject rh;

    public GameObject[] lht;
    public GameObject[] rht;

    public GameObject inventory;
    public GameObject[] slots;

    public void DisableDragHandlers () {
        for (int i = 0; i < 14; ++i) {
            DragHandler D = slots[i].GetComponentInChildren<DragHandler> ();
            if (D != null) {
                D.enabled = false;
            }
        }
    }

    public void EnableDragHandlers () {
        for (int i = 0; i < 14; ++i) {
            DragHandler D = slots[i].GetComponentInChildren<DragHandler> ();
            if (D != null) {
                D.enabled = true;
            }
        }
    }

	// Use this for initialization
	void Start () {
        slots = new GameObject[14];
        int i = 0;
        foreach (Transform t in inventory.transform) {
            slots[i] = t.gameObject;
            ++i;
        }

        lht = new GameObject[3];
        i = 0;
        foreach (Transform t in lh.transform) {
            lht[i] = t.gameObject;
            ++i;
        }

        rht = new GameObject[3];
        i = 0;
        foreach (Transform t in rh.transform) {
            rht[i] = t.gameObject;
            ++i;
        }
	}

    bool findingPlayer = true;
    void Update () {
        if (findingPlayer) {
            GameObject tempPlayer = GameObject.Find ("Player(Clone)");
            if (tempPlayer) {
                character = tempPlayer.GetComponent<Character> ();
                // Debug.Log ("Found Player.");
                findingPlayer = false;
            }
        }
    }


    public void readFromCharacterToInventory () {
        for (int i = 0; i < 3; ++i) {
            NewChildL (i);
            NewChildR (i);
        }
        for (int j = 0; j < 14; ++j) {
            if (character.loot[j] != null) {
                GameObject g =  Instantiate (slotItemPrefab, slots[j].transform) as GameObject;
                g.GetComponent<Text> ().text = character.loot[j].equipmentName + " x" + character.itemCount[j];
                Pickup ce = g.GetComponent<Pickup> ();
                ce.inventoryIndex = j;
                ce.pickupData = character.loot[j];
                slots[j].GetComponentInChildren<Pickup> ().count = character.itemCount[j];
            }
        }
    }

    public void readFromInventoryToCharacter () {

        character.leftHand[3].active = true;
        character.rightHand[3].active = true;

        character.leftHandIndex = 3;
        character.rightHandIndex = 3;

        foreach (Transform t in character.transform) {
            Pickup g = t.gameObject.GetComponent<Pickup> ();
            if (g != null) {
                Destroy (t.gameObject);
            }
        }

        for (int i = 0; i < 3; ++i) {
            if (lht[i].transform.childCount != 0) {
                GameObject go = new GameObject ();
                Pickup ce = go.AddComponent<Pickup> ().GetComponent<Pickup> ();
                ce.pickupData = lht[i].GetComponentInChildren<Pickup> ().pickupData;
                character.leftHandItemCount[i] = lht[i].GetComponentInChildren<Pickup> ().count;
                character.leftHand[i] = ce;
                ce.GetComponent<Pickup> ().inventoryIndex = i;
                ce.transform.parent = character.transform;
            }
            else {
                character.leftHand[i] = null;
            }
        }

        for (int i = 0; i < 3; ++i) {
            if (rht[i].transform.childCount != 0) {
                GameObject go = new GameObject ();
                Pickup ce = go.AddComponent<Pickup> ().GetComponent<Pickup> ();
                ce.pickupData = rht[i].GetComponentInChildren<Pickup> ().pickupData;
                character.rightHandItemCount[i] = rht[i].GetComponentInChildren<Pickup> ().count;
                character.rightHand[i] = ce;
                ce.GetComponent<Pickup> ().inventoryIndex = i;
                ce.transform.parent = character.transform;
            }
            else {
                character.rightHand[i] = null;
            }
        }

        for (int i = 0; i < 14; ++i) {
            character.loot[i] = null;
            if (slots[i].transform.childCount != 0) {
                character.loot[i] = (slots[i].GetComponentInChildren<Pickup> ().pickupData);
                slots[i].GetComponentInChildren<Pickup> ().inventoryIndex = i;
                character.itemCount[i] = slots[i].GetComponentInChildren<Pickup> ().count;
            }
        }

        for (int i = 0; i < 3; ++i) {
            if (rht[i].transform.childCount == 0) {
                character.rightHandItemCount[i] = 0;
            }
            if (lht[i].transform.childCount == 0) {
                character.leftHandItemCount[i] = 0;
            }
        }

        for (int i = 0; i < 14; ++i) {
            if (slots[i].transform.childCount == 0) {
                character.itemCount[i] = 0;
            }
        }

        character.updateGuiText ();
    }

    public void deleteUiElements () {
        for (int i = 0; i < 3; ++i) {
            if (lht[i].transform.childCount != 0) Destroy (lht[i].transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < 3; ++i) {
            if (rht[i].transform.childCount != 0) Destroy (rht[i].transform.GetChild(0).gameObject);
        }

        for (int i = 0; i < 14; ++i) {
            if (slots[i].transform.childCount != 0) {
                Destroy (slots[i].transform.GetChild(0).gameObject);
            }
        }

        // TODO Total hack fix this later.
        GameObject ButtonToDelete = GameObject.Find ("CountDisplay(Clone)");
        if (ButtonToDelete != null) {
            Destroy (ButtonToDelete);
        }


    }

    public void NewChildL (int j) {
        if (character.leftHand[j] != null) {
            GameObject g =  Instantiate (slotItemPrefab, lht[j].transform) as GameObject;
            g.GetComponent<Text>().text = character.leftHand[j].pickupData.equipmentName + " x" + character.leftHandItemCount[j];
            Pickup ce = g.GetComponent<Pickup> ();
            ce.pickupData = character.leftHand[j].pickupData;
            ce.inventoryIndex = j;
            lht[j].GetComponentInChildren<Pickup> ().count = character.leftHandItemCount[j];
        }
    }

    void NewChildR (int j) {
        if (character.rightHand[j] != null) {
            GameObject g =  Instantiate (slotItemPrefab, rht[j].transform) as GameObject;
            g.GetComponent<Text>().text = character.rightHand[j].pickupData.equipmentName + " x" + character.rightHandItemCount[j];
            Pickup ce = g.GetComponent<Pickup> ();
            ce.pickupData = character.rightHand[j].pickupData;
            ce.inventoryIndex = j;
            rht[j].GetComponentInChildren<Pickup> ().count = character.rightHandItemCount[j];
        }
    }

    public void UpdateGuiCounts () {
        for (int i = 0; i < 14; ++i) {
            Text t = slots[i].GetComponentInChildren<Text> ();
            if (t) {
                t.text = character.loot[i].equipmentName + " x" + character.itemCount[i];
            }
        }
    }
}