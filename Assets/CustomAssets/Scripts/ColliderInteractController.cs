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

        RaycastHit hitInfo;

        Ray viewRayCast = Camera.main.ScreenPointToRay (Input.mousePosition);


        if (allowedToPickThingsUp) {
            if (Physics.Raycast(viewRayCast, out hitInfo, 2f)) {
                currentCollider = hitInfo.collider;
                Component component = currentCollider.GetComponent (typeof (IInteractable));
                if (component != null) {
                    IInteractable iInteract = component as IInteractable;
                    if (iInteract != null) {
                        DisplayWhatWasPickedUp (iInteract.ToolTip ());
                    }
                }
            }
            else {
                currentCollider = null;
                // TODO A better way.
                DestroyWhatWasPickedUp ();
            }
        }

        getInput ();

        if (!interactWithCollider)
            return;
        if (currentCollider == null)
            return;
        if (!allowedToPickThingsUp)
            return;

        // Incase we step outside of the zone and our reference goes null.
        copy = currentCollider;

        DeletePrompts ();

        Component comp = copy.GetComponent (typeof (IInteractable));
        IInteractable i = comp as IInteractable;
        i.Interact (gameObject);
	}

    public void DisplayWhatWasPickedUp (string whatWasPickedUp) {
        uiController.CreateWhatWasPickedUp (whatWasPickedUp);
    }

    public void DestroyWhatWasPickedUp () {
        uiController.DestoryWhatWasPickedUp ();
    }

    private void createPopUp (Collider collider) {
        if (!popupInstantiated && allowedToPickThingsUp) {
            popupInstantiated = true;
            newPickupTextPrompt = Instantiate (PickupTextPrompt) as GameObject;
            newPickupTextPrompt.transform.SetParent (GuiCanvas.transform);
            newPickupTextPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
        }
    }

    private void getInput () {
        interactWithCollider = Input.GetKeyDown (KeyCode.F);
    }

    public void DeletePrompts () {
        Destroy (newTalkPrompt);
        Destroy (newPickupTextPrompt);
        popupInstantiated = false;
    }
}
