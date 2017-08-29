using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class allows the player to pick things up
// and manages what you have equipped.
public class Character : MonoBehaviour {

    int INVENTORY_SIZE = 14;

    // Under certain circumstances you are not allowed to pick things up.
    // For example when you have the inventory UI enabled.
    public bool allowedToPickThingsUp;

    // You can hold 3 weapons in each hand.
    // The third index of your hand slots is always
    // the bare hand.
    int NUM_SLOTS_PER_HAND = 4;

    // The current index if the equipped weapon.
    public int leftHandIndex;
    public int rightHandIndex;

    bool usingWeaponInTwoHands;

    public Pickup[] leftHand;
    public Pickup[] rightHand;

    // Not implemented yet.
    public Pickup[] armor;

    // When you pickup an item, its data is stored
    // in this array.
    public PickupData[] loot;

    // The text that appears in the bottom right of the screen.
    // Currently not in use.
    public Text equipped;

    // TODO REDO
    public int[] itemCount;
    public int[] leftHandItemCount;
    public int[] rightHandItemCount;

    public GameObject JustPickedUp;

    // This is the collider you are interacting with when you press 'f'.
    public Collider currentCollider;

    // False if a popup has not been instantiaed..
    public bool alreadyInstantiated = false;
    // Copy of currentCollider for processing when 'f' is pressed.
    Collider copy;

    // Use this for initialization
    void Start () {
        // You should always have your left hand and right hand in the array.
        Debug.Assert (leftHand[3] != null);
        Debug.Assert (rightHand[3] != null);

        allowedToPickThingsUp = true;

        // For the items in your hands, set the transform parent.
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                if (leftHand[i].GetComponent<MeshRenderer> () != null) {
                    leftHand[i].GetComponent<MeshRenderer> ().enabled = false;
                    leftHand[i].gameObject.transform.SetParent (transform);
                }
            }
            if (rightHand[i] != null) {
                if (rightHand[i].GetComponent<MeshRenderer> () != null) {
                    rightHand[i].GetComponent<MeshRenderer> ().enabled = false;
                    rightHand[i].gameObject.transform.SetParent (transform);
                }
            }
        }

        for (int i = 0 ;; ++i) {
            if (leftHand[i] != null) {
                leftHandIndex = i;
                break;
            }
        }
        for (int i = 0 ;; ++i) {
            if (rightHand[i] != null) {
                rightHandIndex = i;
                break;
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                leftHand[i].active = true;
                break;
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].active = true;
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
	}


    // Update is called once per frame
    void Update () {

        if (Input.GetKeyDown (KeyCode.F)) {

            if (currentCollider == null) return;
            if (!allowedToPickThingsUp) return;

            // Incase we step outside of the zone and our reference goes null.
            copy = currentCollider;

            alreadyInstantiated = false;
            currentCollider = null;

            if (copy.tag == "Quest") {
                gameObject.GetComponent<TalkHandler> ().DeletePrompt ();
                QuestLogic ();
                return;
            }
            else if (copy.tag == "Pickup") {
                gameObject.GetComponent<PickupHandler>().DeletePrompt();
                PickupLogic ();

                return;
            }
        }

        // 1 to switch weapons in your left hand.
        if (Input.GetKeyDown (KeyCode.Alpha1)) {

            if (usingWeaponInTwoHands) {
                usingWeaponInTwoHands = false;
                if (rightHand[rightHandIndex] != null) {
                    rightHand[rightHandIndex].active = true;
                }
            }

            bool nothing = (leftHand[0] == null && leftHand[1] == null && leftHand[2] == null);
            if (nothing) {
                return;
            }

            bool findingWeapon = true;
            int previousIndex = leftHandIndex;
            do {
                leftHandIndex++;

                if (leftHandIndex >= NUM_SLOTS_PER_HAND) {
                    leftHandIndex = 0;
                }

                if (leftHand[leftHandIndex] != null) {
                    leftHand[leftHandIndex].active = true;
                    if (leftHandIndex != previousIndex) {
                        leftHand[previousIndex].active = false;
                    }
                    findingWeapon = false;
                }
            } while (findingWeapon) ;

            for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                if (i != leftHandIndex && leftHand[i] != null) {
                    leftHand[i].active = false;
                }
            }

            if (leftHand[leftHandIndex].pickupData.handOccupancy == HandOccupancies.JustTwoHanded) {
                disableRightHand ();
            }
            // updateGuiText ();
        }

        // 2 to switch weapons in your right hand.
        if (Input.GetKeyDown (KeyCode.Alpha2)) {

            if (usingWeaponInTwoHands) {
                usingWeaponInTwoHands = false;
                if (leftHand[leftHandIndex] != null) {
                    leftHand[leftHandIndex].active = true;
                }
            }

            bool nothing = (rightHand[0] == null && rightHand[1] == null && rightHand[2] == null);
            if (nothing) {
                return;
            }

            bool findingWeapon = true;
            int previousIndex = rightHandIndex;
            do {
                rightHandIndex++;

                if (rightHandIndex >= NUM_SLOTS_PER_HAND) {
                    rightHandIndex = 0;
                }

                if (rightHand[rightHandIndex] != null) {
                    rightHand[rightHandIndex].active = true;
                    if (previousIndex != rightHandIndex) {
                        rightHand[previousIndex].active = false;
                    }
                    findingWeapon = false;
                }
            } while (findingWeapon) ;

            for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                if (i != rightHandIndex && rightHand[i] != null) {
                    rightHand[i].active = false;
                }
            }

            if (rightHand[rightHandIndex].pickupData.handOccupancy == HandOccupancies.JustTwoHanded) {
                disableLeftHand ();
            }
            // updateGuiText ();
        }

        // Q to two hand your left hand weapon.
        if (Input.GetKeyDown (KeyCode.Q)) {
            if (usingWeaponInTwoHands) {
                bool twoHandingRightWeapon = false;
                for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                    if (rightHand[i] != null) {
                        if (rightHand[i].active) {
                            twoHandingRightWeapon = true;
                        }
                    }
                }

                if (twoHandingRightWeapon) {
                    disableRightHand ();
                    leftHand[leftHandIndex].active = true;
                    // updateGuiText ();
                    return;
                } 
            }

            if (!usingWeaponInTwoHands) {
                disableRightHand ();
                usingWeaponInTwoHands = true;
            }
            else {
                usingWeaponInTwoHands = false;
                if (rightHand[rightHandIndex] != null) {
                    rightHand[rightHandIndex].active = true;
                }
            }
            // updateGuiText ();
        }

        // E to two hand your right hand weapon.
        if (Input.GetKeyDown (KeyCode.E)) {
            if (usingWeaponInTwoHands) {
                bool twoHandingLeftWeapon = false;
                for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                    if (leftHand[i] != null) {
                        if (leftHand[i].active) {
                            twoHandingLeftWeapon = true;
                        }
                    }
                }

                if (twoHandingLeftWeapon) {
                    disableLeftHand();
                    rightHand[rightHandIndex].active = true;
                    // updateGuiText ();
                    return;
                } 
            }

            if (!usingWeaponInTwoHands) {
                disableLeftHand ();
                usingWeaponInTwoHands = true;
            }
            else {
                usingWeaponInTwoHands = false;
                if (leftHand[leftHandIndex] != null) {
                    leftHand[leftHandIndex].active = true;
                }
            }
            // updateGuiText ();
        }
	}

    void PickupLogic () {
        Pickup[] C = copy.gameObject.GetComponents<Pickup> ();
        if (C.Length == 0) return;

        for (int i = 0; i < C.Length; ++i) {
            gameObject.GetComponent<ObjectiveListener> ().ObserveAction (C[i]);
        }

        GameObject popup = Instantiate (JustPickedUp) as GameObject;
        Text t = popup.GetComponent<Text> ();
        popup.SetActive (false);

        int numPickedUp = 0;
        for (int i = 0; i < C.Length; ++i) {
            bool foundSlotForItem = false;
            if (C[i].pickupData.stackable) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (C[i].pickupData == loot[j]) {
                        itemCount[j] += C[i].count;
                        foundSlotForItem = true;
                        t.text += C[i].pickupData.equipmentName + " x" + C[i].count.ToString () + "\n";
                        Destroy (C[i]);
                        numPickedUp++;
                    }
                }
            }
            if (!foundSlotForItem) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (loot[j] == null) {
                        itemCount[j] += C[i].count;
                        loot[j] = C[i].pickupData;
                        t.text += C[i].pickupData.equipmentName + " x" + C[i].count.ToString () + "\n";
                        Destroy (C[i]);
                        numPickedUp++;
                        break;
                    }
                }
            }
        }
        popup.SetActive (true);
        CursorManager cursorManager = gameObject.GetComponent<CursorManager> ();
        cursorManager.cursorLocked = false;
        cursorManager.listening = false;

        PlayerController playerController = GetComponent<PlayerController> ();
        playerController.shouldRotate = false;
        playerController.listening = false;

        popup.transform.SetParent (GameObject.Find("Canvas").transform);
        popup.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
        allowedToPickThingsUp = false;
        gameObject.GetComponent<UIManager> ().enabled = false;
        if (numPickedUp == C.Length) {
            Destroy (copy.gameObject);
        }
        if (numPickedUp == 0) {
            t.text = "Inventory full.";
        }
    }

    void QuestLogic () {
        allowedToPickThingsUp = false;
        GameObject popup = Instantiate (JustPickedUp) as GameObject;
        Text t = popup.GetComponent<Text> ();

        popup.SetActive (true);
        CursorManager cursorManager = gameObject.GetComponent<CursorManager> ();
        cursorManager.cursorLocked = false;
        cursorManager.listening = false;

        PlayerController playerController = GetComponent<PlayerController> ();
        playerController.shouldRotate = false;
        playerController.listening = false;

        popup.transform.SetParent (GameObject.Find("Canvas").transform);
        popup.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
        gameObject.GetComponent<UIManager> ().enabled = false;
        // This is where this script was getting the Quest componenet of the collision
        // and getting the dialog to populate the box with.

        QuestHolder qh = copy.GetComponent<QuestHolder> ();
        Debug.Assert (qh != null);

        if (qh.QuestsIHaveToGive.Length != 0) {
            for (int i = 0; i < qh.given.Length; ++i) {
                if (!qh.given[i]) {
                    GameObject QuestManager = GameObject.Find ("QuestManager");
                    QuestManager.GetComponent<QuestManagerScript> ().QuestsIHave.Add(qh.QuestsIHaveToGive[i]);
                    qh.given[i] = true;
                    t.text = (qh.QuestsIHaveToGive[i].Objectives[0].description);
                    break;
                }
            }
        }
    }

    void disableLeftHand () {
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                leftHand[i].active = false;
            }
        }
    }

    void disableRightHand () {
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].active = false;
            }
        }
    }

    /*
    public void updateGuiText () {
        equipped.text = "";

        string leftHandText = "";
        string rightHandText = "";

        if (usingWeaponInTwoHands) {
            equipped.text += "Two Handed ";
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                if (leftHand[i].active) {
                    leftHandText += leftHand[i].pickupData.equipmentName;
                }
            }
        }
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                if (rightHand[i].active) {
                    rightHandText += rightHand[i].pickupData.equipmentName;
                }
            }
        }
        equipped.text = equipped.text + leftHandText + " " + rightHandText;
    }
    */
}