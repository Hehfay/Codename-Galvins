using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// This class allows the player to pick things up
// and manages what you have equipped.
public class Character : MonoBehaviour {

    int INVENTORY_SIZE = 14; 

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

    // When you pickup and item, its data is stored
    // in this array.
    public PickupData[] loot;

    // The text that appears in the bottom right of the screen.
    public Text equipped;

    // TODO REDO
    public int[] itemCount;
    public int[] leftHandItemCount;
    public int[] rightHandItemCount;

    // Use this for initialization
    void Start () {
        // You should always have your left hand and right hand in the array.
        Debug.Assert (leftHand[3] != null);
        Debug.Assert (rightHand[3] != null);

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
        updateGuiText ();
	}

    // Update is called once per frame
    void Update () {

        // F button to pick up equipment.
        if (Input.GetKeyDown (KeyCode.F)) {
            Vector3 v = transform.TransformDirection (Vector3.forward);
            RaycastHit h;
            Physics.Raycast (transform.position, v, out h, 3, 1);

            if (h.collider != null) {
                Pickup C = h.collider.gameObject.GetComponent<Pickup> ();

                if (C == null) {
                    Debug.Log ("Collided with something that can't be picked up.");
                    return;
                }

                if (C.pickupData.stackable) {
                    for (int i = 0; i < INVENTORY_SIZE; ++i) {
                        if (C.pickupData == loot[i]) {
                            itemCount[i] += C.gameObject.GetComponent<Pickup> ().count;
                            Destroy (C.gameObject);
                            return;
                        }
                    }
                }

                for (int i = 0; i < INVENTORY_SIZE; ++i) {
                    if (loot[i] == null) {
                        itemCount[i] += C.gameObject.GetComponent<Pickup> ().count;
                        loot[i] = C.pickupData;
                        Destroy (C.gameObject);
                        break;
                    }
                }
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
            updateGuiText ();
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
            updateGuiText ();
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
                    updateGuiText ();
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
            updateGuiText ();
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
                    updateGuiText ();
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
            updateGuiText ();
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

    public void updateGuiText () {
        /*
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
        */
    }
}