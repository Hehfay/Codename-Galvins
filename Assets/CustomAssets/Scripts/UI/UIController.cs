using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UIState {
    Quest,
    Inventory,
};

// This script handles toggling the UI.
public class UIController : MonoBehaviour {


    public bool selectCountEnabled;

    private bool openInventory;

    public UIState uiState;

    InventoryController invCont;

    public GameObject lefthand;
    public GameObject righthand;
    public GameObject inventory;
    public GameObject questTab;
    public GameObject inventoryTab;
    public GameObject questLog;
    public GameObject SelectCount;
    public GameObject justPickedup;
    CharacterInventory myCharacterScript;

    bool isActive;

	void Start () {

        // This is probably a good way to do it. I should clean it up with a for each child in transform children loop.
        // And then everything that needs the UI can find the UIController and call public functions on it.

        SelectCount.SetActive (false);
        isActive = false;
        lefthand.SetActive (false);
        righthand.SetActive (false);
        inventory.SetActive (false);
        questTab.SetActive (false);
        inventoryTab.SetActive (false);
        questLog.SetActive (false);
        justPickedup.SetActive (false);
        invCont = GetComponent<InventoryController>();
	}
	
	// Update is called once per frame
	void Update () {

        getInput ();

        if (openInventory) {
            DestoryWhatWasPickedUp ();

            // Toggle the inventory display.
            isActive = !isActive;

            GameObject.Find("Player(Clone)").GetComponent<ColliderInteractController>().allowedToPickThingsUp = !isActive;
            GameObject.Find ("Player(Clone)").GetComponent<ColliderInteractController> ().DeletePrompts ();

            if (isActive) {
                GameObject.Find ("Player(Clone)").GetComponent<CursorManager> ().cursorLocked = false;
                GameObject.Find ("Player(Clone)").GetComponent<PlayerMovementController> ().shouldRotate = false;
                switch (uiState) {
                    case UIState.Inventory:
                    DrawInventory ();
                    break;

                    case UIState.Quest:
                    DrawQuestLog ();
                    break;
                }
            }
            else {
                GameObject.Find ("Player(Clone)").GetComponent<CursorManager> ().cursorLocked = true;
                GameObject.Find ("Player(Clone)").GetComponent<PlayerMovementController> ().shouldRotate = true;
                invCont.deleteUiElements ();
                lefthand.SetActive (false);
                righthand.SetActive (false);
                inventory.SetActive (false);
                questTab.SetActive (false);
                questLog.SetActive (false);
                inventoryTab.SetActive (false);
            }
        }
	}

    public void InventoryTabClicked () {
        if (uiState == UIState.Inventory) {
            return;
        }
        DrawInventory ();
        uiState = UIState.Inventory;
    }

    public void QuestTabClicked () {
        if (uiState == UIState.Quest) {
            return;
        }
        invCont.deleteUiElements ();
        DrawQuestLog ();
        uiState = UIState.Quest;
    }

    public void DrawInventory () {

        questTab.SetActive (true);
        inventoryTab.SetActive (true);

        lefthand.SetActive (true);
        righthand.SetActive (true);
        inventory.SetActive (true);

        questLog.SetActive (false);

        invCont.readFromCharacterToInventory ();
    }

    public void DrawQuestLog () {
        questLog.GetComponentInChildren<Text> ().text = "";

        questTab.SetActive (true);
        inventoryTab.SetActive (true);

        lefthand.SetActive (false);
        righthand.SetActive (false);
        inventory.SetActive (false);

        questLog.SetActive (true);
        GetComponent<QuestManager> ().WriteToQuestLog ();
    }

    private void getInput () {
        openInventory = Input.GetKeyDown (KeyCode.E);
    }


    public void CreateSelectCountButton () {
        SelectCount.SetActive (true);
        SelectCount.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
        selectCountEnabled = true;
    }

    public void DestoryCountButton () {
        SelectCount.SetActive (false);
        selectCountEnabled = false;
    }

    public void SelectCountButtonSetText (string s) {
        SelectCount.GetComponentInChildren<Text> ().text = s;
    }

    public void SelectCountButtonSetMaxCount (int count) {
        SelectCount.GetComponentInChildren<PlusButtonOnClick> ().maxCount = count;
    }

    public void CreateWhatWasPickedUp (string whatWasPickedUp) {
        justPickedup.SetActive (true);
        justPickedup.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
        justPickedup.GetComponent<Text> ().text = whatWasPickedUp;

        if (justPickedup.GetComponent<Timer> () != null) {
            justPickedup.GetComponent<Timer> ().duration = 3f;
        }
        else {
            justPickedup.AddComponent<Timer> ().duration = 3f;
        }
    }

    public void DestoryWhatWasPickedUp () {
        justPickedup.SetActive (false);
    }
}