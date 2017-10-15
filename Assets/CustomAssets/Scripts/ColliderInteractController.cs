using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderInteractController : MonoBehaviour {

    GameObject GuiCanvas;

    // The popup of what you just picked up.
    public GameObject JustPickedUp;

    public GameObject PickupTextPrompt;
    GameObject newPickupTextPrompt;

    public bool allowedToPickThingsUp;

    public GameObject TalkPrompt;
    GameObject newTalkPrompt;

    // This is the collider you are interacting with when you press 'f'.
    public Collider currentCollider;

    // Copy of currentCollider for processing when 'f' is pressed.
    Collider copy;

    public bool interactWithCollider;

    // False if a popup has not been instantiaed.
    public bool popupInstantiated = false;

    UIController uiController;

	// Use this for initialization
	void Start () {
        allowedToPickThingsUp = true;
        GuiCanvas = GameObject.Find ("Canvas");
        uiController = GuiCanvas.GetComponent<UIController> ();
	}
	
	// Update is called once per frame
	void Update () {

        //////////
        // TODO //
        //////////
        // Handle two colliders at once.
        // We need the ability to switch between talking to someone and
        // picking something up.

        getInput ();

        if (!interactWithCollider)
            return;
        if (currentCollider == null)
            return;
        if (!allowedToPickThingsUp)
            return;

        // Incase we step outside of the zone and our reference goes null.
        copy = currentCollider;
        allowedToPickThingsUp = false;

        switch (copy.tag) {
            case "Pickup": {
                DeletePrompts ();
                GetComponent<CharacterInventory> ().PickupLogic (copy);
            } break;

            case "Quest": {
                DeletePrompts ();

                // TODO Dialog trees.
                DisplayWhatWasPickedUp ("");
            } break;
        }
	}

    public void DisplayWhatWasPickedUp (string whatWasPickedUp) {
        //LockView ();
        allowedToPickThingsUp = true;

        uiController.CreateWhatWasPickedUp (whatWasPickedUp);

        // TODO Another way to detect nothing was picked up.
        // if (whatWasPickedUp != "") {
        GetComponent<QuestManager> ().ProcessQuestTrigger (copy.GetComponent<QuestTrigger> ());
    }

    void OnTriggerEnter (Collider collider) {

        if (!popupInstantiated && allowedToPickThingsUp) {
            switch (collider.tag) {

                case "Pickup": {
                    popupInstantiated = true;
                    newPickupTextPrompt = Instantiate (PickupTextPrompt) as GameObject;
                    newPickupTextPrompt.transform.SetParent (GuiCanvas.transform);

                    newPickupTextPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
                } break;

                case "Quest": {
                    popupInstantiated = true;
                    newTalkPrompt = Instantiate (TalkPrompt) as GameObject;
                    newTalkPrompt.transform.SetParent (GuiCanvas.transform);
                    newTalkPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
                } break;
            }
            currentCollider = collider;
        }
    }

    void OnTriggerStay (Collider collider) {
        if (!popupInstantiated && allowedToPickThingsUp) {
            switch (collider.tag) {

                case "Pickup": {
                    popupInstantiated = true;
                    newPickupTextPrompt = Instantiate (PickupTextPrompt) as GameObject;
                    newPickupTextPrompt.transform.SetParent (GuiCanvas.transform);
                    newPickupTextPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
                } break;

                case "Quest": {
                    popupInstantiated = true;
                    newTalkPrompt = Instantiate (TalkPrompt) as GameObject;
                    newTalkPrompt.transform.SetParent (GuiCanvas.transform);
                    newTalkPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
                } break;
            }
            currentCollider = collider;
        }
    }

    void OnTriggerExit (Collider collider) {
        DeletePrompts ();
    }

    private void getInput () {
        interactWithCollider = Input.GetKeyDown (KeyCode.F);
    }

    public void DeletePrompts () {
        Destroy (newTalkPrompt);
        Destroy (newPickupTextPrompt);
        popupInstantiated = false;
    }

    private void LockView () {
        CursorManager cursorManager = gameObject.GetComponent<CursorManager> ();
        PlayerMovementController playerController = GetComponent<PlayerMovementController> ();
        cursorManager.cursorLocked = false;
        cursorManager.listening = false;
        playerController.shouldRotate = false;
        allowedToPickThingsUp = false;
    }
}
