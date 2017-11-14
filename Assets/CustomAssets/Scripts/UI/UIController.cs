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

    public GameObject DialogBox;
    public GameObject DialogTopics;
    public GameObject NameBox;
    public GameObject GoodbyeButton;

    public GameObject DialogTopicButton;

    public GameObject Layout;

    Inventory myCharacterScript;

    /* I am just going to see how this goes. */
    private GameObject CharacterThatActivatedDialog;

    /* And seeing how this goes. */
    private GameObject CurrentInteraction;

    bool isActive;

	void Start () {
        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild (i).transform.gameObject.SetActive (false);
        }
        invCont = GetComponent<InventoryController>();
	}
	
	// Update is called once per frame
	void Update () {

        getInput ();

        if (openInventory) {
            DestoryWhatWasPickedUp ();

            // Toggle the inventory display.
            isActive = !isActive;

            GameObject.Find("A03(Clone)").GetComponent<ColliderInteractController>().allowedToPickThingsUp = !isActive;
            GameObject.Find ("A03(Clone)").GetComponent<ColliderInteractController> ().DeletePrompts ();

            if (isActive) {
                GameObject.Find ("A03(Clone)").GetComponent<CursorManager> ().cursorLocked = false;
                GameObject.Find ("A03(Clone)").GetComponent<PlayerMovementController> ().shouldRotate = false;
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
                GameObject.Find ("A03(Clone)").GetComponent<CursorManager> ().cursorLocked = true;
                GameObject.Find ("A03(Clone)").GetComponent<PlayerMovementController> ().shouldRotate = true;
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

        GameObject.Find ("QuestManager").GetComponent<QuestManager> ().WriteToQuestLog ();
        // GetComponent<QuestManager> ().WriteToQuestLog ();
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

        if (whatWasPickedUp == "") {
            whatWasPickedUp = "Inventory Full";
        }
        justPickedup.GetComponentInChildren<Text> ().text = whatWasPickedUp;
    }

    public void DestoryWhatWasPickedUp () {
        justPickedup.SetActive (false);
    }

    /******************/
    /* Dialog Methods */
    /******************/

    public void SetDialogBoxActiveState (bool state) {
        DialogBox.SetActive(state);
        DialogTopics.SetActive(state);
        NameBox.SetActive (state);
        GoodbyeButton.SetActive (state);
    }

    private void SetCharacterRotateState (bool state) {
        CharacterThatActivatedDialog.GetComponent<CursorManager> ().cursorLocked = state;
        CharacterThatActivatedDialog.GetComponent<PlayerMovementController> ().shouldRotate = state;
    }

    public void EnableDialog (GameObject characterThatActivatedDialog, GameObject interaction) {
        CharacterThatActivatedDialog = characterThatActivatedDialog;
        CurrentInteraction = interaction;
        SetCharacterRotateState (false);
        SetDialogBoxActiveState (true);

        DisplayName (interaction.GetComponent<AIInteraction>().characterName);
        DisplayGreeting (interaction.GetComponent<AIDialogContainer>().greeting);

        int iMax = interaction.GetComponent<AIDialogContainer> ().dialogTopics.Length;
        for (int i = 0; i < iMax; ++i) {
            GameObject obj = Instantiate (DialogTopicButton);
            obj.GetComponent<DialogTopicOnClick> ().questManager = characterThatActivatedDialog.GetComponent<QuestManager> ();
            obj.transform.SetParent(Layout.transform, true);
            obj.GetComponent<DialogTopicOnClick> ().dialogTopic = interaction.GetComponent<AIDialogContainer> ().dialogTopics[i];
            obj.GetComponentInChildren<Text> ().text = interaction.GetComponent<AIDialogContainer> ().dialogTopics[i];
            obj.GetComponent<DialogTopicOnClick> ().dialog = interaction.GetComponent<AIDialogContainer> ().dialog[i];
            obj.GetComponent<DialogTopicOnClick> ().questTrigger = interaction.GetComponent<AIDialogContainer> ().questTriggers[i];
        }
    }

    public void DisableDialog () {
        SetCharacterRotateState (true);
        SetDialogBoxActiveState (false);
        CharacterThatActivatedDialog = null;
    }

    public void DisplayGreeting (string greeting) {
        DialogBox.GetComponentInChildren<Text> ().text = greeting;
    }

    public void DisplayName (string name) {
        NameBox.GetComponent<Text> ().text = name;
    }

    public void DialogOptionClicked (string dialog) {
        DialogBox.GetComponentInChildren<Text> ().text = dialog;
    }

    public void DestroyTopics () {
        DialogTopicOnClick[] objsToDelete = DialogTopics.GetComponentsInChildren<DialogTopicOnClick> ();
        for (int i = 0; i < objsToDelete.Length; ++i) {
            Destroy (objsToDelete[i].gameObject);
        }
    }
}