using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColliderInteractController : MonoBehaviour {
    public GameObject InteractPrompt;
    public bool allowedToPickThingsUp;

    private GameObject Canvas;
    private Collider currentCollider;
    private Collider previousCollider;
    private List<GameObject> references;
    private bool popupInstantiated;
    private bool interactInput;

    // This value could be changed by a telekenesis effect, perhaps.
    // Set in the editor.
    public float rayCastDistance;

    private void DestroyPopup () {
        Destroy (references[0]);
        references.Clear ();
        popupInstantiated = false;
    }

    private void CreatePopup (string popUpText) {
        references.Clear ();

        GameObject popup = (Instantiate (InteractPrompt, Canvas.transform, false));
        references.Add (popup);
        references[0].transform.GetChild (0).transform.GetChild (0).GetComponent<Text> ().text = popUpText;
        popupInstantiated = true;
    }

	void Start () {
        references = new List<GameObject>();
        Canvas = GameObject.Find ("Canvas");
        allowedToPickThingsUp = true;
        popupInstantiated = false;
        Debug.Assert (Canvas != null);
        currentCollider = previousCollider = null;
	}

    private void GetInput () {
        // allowedToPickThingsUp = (Input.GetKeyDown (KeyCode.E) ^ allowedToPickThingsUp);
        interactInput = Input.GetKeyDown (KeyCode.F);
    }
	
	void Update () {

        GetInput ();

        RaycastHit hitInfo;
        Ray viewRayCast = Camera.main.ScreenPointToRay (Input.mousePosition);

        if (allowedToPickThingsUp) {

            if (Physics.Raycast (viewRayCast, out hitInfo, rayCastDistance)) {
                previousCollider = currentCollider;
                currentCollider = hitInfo.collider;
                Component component = currentCollider.GetComponent (typeof (IInteractable));

                if (component != null) {
                    IInteractable iInteract = component as IInteractable;
                    if (iInteract != null && !popupInstantiated) {
                        CreatePopup (iInteract.ToolTip ());
                    }
                    else if (currentCollider != previousCollider) {
                        // Make a brand new one.
                        DestroyPopup ();
                        CreatePopup (iInteract.ToolTip ());
                    }
                }
                else {
                    previousCollider = currentCollider = null;
                    if (popupInstantiated) {
                        DestroyPopup ();
                    }
                }
            }
            else {
                previousCollider = currentCollider = null;
                if (popupInstantiated) {
                    DestroyPopup ();
                }
            }
        }
        else {
            previousCollider = currentCollider = null;
            if (popupInstantiated) {
                DestroyPopup ();
            }
        }

        if (popupInstantiated && allowedToPickThingsUp && currentCollider != null && interactInput) {
            Component comp = currentCollider.GetComponent (typeof (IInteractable));
            if (comp == null) {
                currentCollider = previousCollider = null;
                DestroyPopup ();
                return;
            }
            IInteractable i = comp as IInteractable;
            i.Interact (gameObject);
        }
        // No further functionality with the object your interacting with should be called here.
        // Everything should be in the Interact function.
	}

    public void DestroyPopUpConditionally () {
        if (popupInstantiated) {
            DestroyPopup ();
        }
    }

    public void DisableInteractController () {
        enabled = false;
        DestroyPopUpConditionally ();
        allowedToPickThingsUp = false;
    }

    public void EnableInteractController () {
        enabled = true;
        allowedToPickThingsUp = true;
    }
}