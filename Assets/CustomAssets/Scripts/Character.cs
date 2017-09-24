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

    public GameObject[] leftHand;
    public GameObject[] rightHand;

    // Not implemented yet.
    public Pickup[] armor;

    // When you pickup an item, its data is stored
    // in this array.
    public DataSheet[] loot;

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

        if (leftHand[3] == null) {
            leftHand[3] = Resources.Load<GameObject> ("Items/LeftHand");
        }

        if (rightHand[3] == null) {
            rightHand[3] = Resources.Load<GameObject> ("Items/RightHand");
        }


        allowedToPickThingsUp = true;

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
                leftHand[i].GetComponent<Pickup>().active = true;
                break;
            }
        }

        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].GetComponent<Pickup>().active = true;
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

            // TODO Instead of the tag it is going to be a signal from the collider interaction brain.
            if (copy.tag == "Pickup") {
                gameObject.GetComponent<PickupHandler>().DeletePrompt();
                PickupLogic ();
                return;
            }

            if (copy.tag == "Quest") {
                gameObject.GetComponent<TalkHandler> ().DeletePrompt ();
                GameObject popup = Instantiate (JustPickedUp) as GameObject;
                CursorManager cursorManager = gameObject.GetComponent<CursorManager> ();
                PlayerController2 playerController = GetComponent<PlayerController2> ();
                cursorManager.cursorLocked = false;
                cursorManager.listening = false;
                playerController.shouldRotate = false;
                allowedToPickThingsUp = false;

                QuestManagerScript qms = GameObject.Find ("QuestManager").GetComponent<QuestManagerScript>();
                bool doOtherThing = true;

                for (int i = 0; i < qms.ActiveQuests.Count; ++i) {
                    if (copy.gameObject.GetComponent<QuestWrapper>().quest == qms.ActiveQuests[i].quest &&
                        !qms.ActiveQuests[i].isActiveQuest) {
                        qms.ActiveQuests[i].isActiveQuest = true;

                        qms.ActiveQuests[i].currentObjective = qms.ActiveQuests[i].quest.firstObjective;

                        bool foundCurrentObjective = false;

                        while (!foundCurrentObjective) {

                            bool currentObjectiveComplete = true;

                            QuestWrapper qw = copy.gameObject.GetComponent<QuestWrapper> ();

                            qms.getQuest (qw.quest).ObjectiveCompletionStatus.TryGetValue(qms.getQuest(qw.quest).currentObjective, out currentObjectiveComplete);

                            if (currentObjectiveComplete) {
                                qms.getQuest(qw.quest).ObjectiveCompletionStatus.Remove (qms.getQuest(qw.quest).currentObjective);
                                qms.getQuest(qw.quest).ObjectiveCompletionStatus.Add (qms.getQuest(qw.quest).currentObjective, true);
                                qms.getQuest (qw.quest).NextObjective ();
                                doOtherThing = false;
                            }
                            else {
                                foundCurrentObjective = true;
                            }
                        }
                        qms.ShowCurrentObjectives ();
                    }
                }

                if (doOtherThing) {
                    // TODO If the tag is quest, the quest trigger will be in the quest trigger wrapper, not the pickupData.
                    QuestTrigger qt = copy.gameObject.GetComponent<QuestTriggerWrapper> ().questTrigger;
                    if (qt != null) {
                        for (int j = 0; j < qms.ActiveQuests.Count; ++j) {
                            if (qt.quest == qms.ActiveQuests[j].quest) {

                                bool satisfied = false;

                                qms.ActiveQuests[j].ObjectiveCompletionStatus.TryGetValue (qt.advanceCondition, out satisfied);

                                bool currentObjectiveComplete = true;

                                qms.ActiveQuests[j].ObjectiveCompletionStatus.TryGetValue (qms.ActiveQuests[j].currentObjective, out currentObjectiveComplete);

                                currentObjectiveComplete = qms.ActiveQuests[j].currentObjective == qt.nextObjective;

                                if (satisfied && !currentObjectiveComplete) {

                                    qms.ActiveQuests[j].ObjectiveCompletionStatus.Remove (qms.ActiveQuests[j].currentObjective);
                                    qms.ActiveQuests[j].ObjectiveCompletionStatus.Add (qms.ActiveQuests[j].currentObjective, true);

                                    qms.ActiveQuests[j].currentObjective = qt.nextObjective;
                                    qms.ShowCurrentObjectives ();
                                }
                            }
                        }
                    }
                }


                popup.transform.SetParent (GameObject.Find("Canvas").transform);
                popup.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
                gameObject.GetComponent<UIManager> ().enabled = false;
            }
        }

        // 1 to switch weapons in your left hand.
        if (Input.GetKeyDown (KeyCode.Alpha1)) {

            if (usingWeaponInTwoHands) {
                usingWeaponInTwoHands = false;
                if (rightHand[rightHandIndex] != null) {
                    rightHand[rightHandIndex].GetComponent<Pickup>().active = true;
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
                    leftHand[leftHandIndex].GetComponent<Pickup>().active = true;
                    if (leftHandIndex != previousIndex) {
                        leftHand[previousIndex].GetComponent<Pickup>().active = false;
                    }
                    findingWeapon = false;
                }
            } while (findingWeapon) ;

            for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                if (i != leftHandIndex && leftHand[i] != null) {
                    leftHand[i].GetComponent<Pickup>().active = false;
                }
            }

            if (leftHand[leftHandIndex].GetComponent<DataSheetWrapper>().dataSheet.handOccupancy == HandOccupancies.JustTwoHanded) {
                disableRightHand ();
            }
            // updateGuiText ();
        }

        // 2 to switch weapons in your right hand.
        if (Input.GetKeyDown (KeyCode.Alpha2)) {

            if (usingWeaponInTwoHands) {
                usingWeaponInTwoHands = false;
                if (leftHand[leftHandIndex] != null) {
                    leftHand[leftHandIndex].GetComponent<Pickup>().active = true;
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
                    rightHand[rightHandIndex].GetComponent<Pickup>().active = true;
                    if (previousIndex != rightHandIndex) {
                        rightHand[previousIndex].GetComponent<Pickup>().active = false;
                    }
                    findingWeapon = false;
                }
            } while (findingWeapon) ;

            for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
                if (i != rightHandIndex && rightHand[i] != null) {
                    rightHand[i].GetComponent<Pickup>().active = false;
                }
            }

            if (rightHand[rightHandIndex].GetComponent<DataSheetWrapper>().dataSheet.handOccupancy == HandOccupancies.JustTwoHanded) {
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
                        if (rightHand[i].GetComponent<Pickup>().active) {
                            twoHandingRightWeapon = true;
                        }
                    }
                }

                if (twoHandingRightWeapon) {
                    disableRightHand ();
                    leftHand[leftHandIndex].GetComponent<Pickup>().active = true;
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
                    rightHand[rightHandIndex].GetComponent<Pickup>().active = true;
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
                        if (leftHand[i].GetComponent<Pickup>().active) {
                            twoHandingLeftWeapon = true;
                        }
                    }
                }
                if (twoHandingLeftWeapon) {
                    disableLeftHand();
                    rightHand[rightHandIndex].GetComponent<Pickup>().active = true;
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
                    leftHand[leftHandIndex].GetComponent<Pickup>().active = true;
                }
            }
            // updateGuiText ();
        }
	}

    void PickupLogic () {
        DataSheetWrapper[] dataSheetWrapper = copy.gameObject.GetComponents<DataSheetWrapper> ();
        Pickup[] pickup = copy.gameObject.GetComponents<Pickup> ();
        if (dataSheetWrapper.Length == 0) {
            return;
        }

        GameObject popup = Instantiate (JustPickedUp) as GameObject;
        Text t = popup.GetComponent<Text> ();
        popup.SetActive (false);

        int numPickedUp = 0;
        for (int i = 0; i < dataSheetWrapper.Length; ++i) {
            bool foundSlotForItem = false;
            if (dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.stackable) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet == loot[j]) {
                        itemCount[j] += pickup[i].count;
                        foundSlotForItem = true;
                        t.text += dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.equipmentName + " x" + pickup[i].count.ToString () + "\n";
                        Destroy (dataSheetWrapper[i]);
                        numPickedUp++;
                    }
                }
            }
            if (!foundSlotForItem) {
                for (int j = 0; j < INVENTORY_SIZE; ++j) {
                    if (loot[j] == null) {
                        itemCount[j] += pickup[i].count;
                        loot[j] = dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet;
                        t.text += dataSheetWrapper[i].GetComponent<DataSheetWrapper>().dataSheet.equipmentName + " x" + pickup[i].count.ToString () + "\n";
                        Destroy (dataSheetWrapper[i]);
                        numPickedUp++;
                        break;
                    }
                }
            }

            // See if the item has a quest trigger.
            QuestTrigger qt = copy.gameObject.GetComponent<Pickup> ().GetComponent<DataSheetWrapper>().dataSheet.questTrigger;
            if (qt != null) {
                QuestManagerScript qms = GameObject.Find ("QuestManager").GetComponent<QuestManagerScript> ();
                for (int j = 0; j < qms.ActiveQuests.Count; ++j) {
                    if (qt.quest == qms.ActiveQuests[j].quest) {

                        if (!qms.ActiveQuests[j].isActiveQuest) {
                            qms.ActiveQuests[j].ObjectiveCompletionStatus.Remove (qt.thisObjective);
                            qms.ActiveQuests[j].ObjectiveCompletionStatus.Add (qt.thisObjective, true);
                        }
                        else {
                            bool conditionMet = false;
                            qms.ActiveQuests[j].ObjectiveCompletionStatus.TryGetValue (qt.advanceCondition, out conditionMet);

                            bool currentObjectiveComplete;
                            qms.ActiveQuests[j].ObjectiveCompletionStatus.TryGetValue (qms.ActiveQuests[j].currentObjective, out currentObjectiveComplete);


                            if (conditionMet && !currentObjectiveComplete) {
                                qms.ActiveQuests[j].ObjectiveCompletionStatus.Remove (qms.ActiveQuests[j].currentObjective);
                                qms.ActiveQuests[j].ObjectiveCompletionStatus.Add (qms.ActiveQuests[j].currentObjective, true);

                                qms.ActiveQuests[j].currentObjective = qt.nextObjective;
                                qms.ShowCurrentObjectives ();
                            }
                        }
                    }
                }
            }
        }
        popup.SetActive (true);
        CursorManager cursorManager = gameObject.GetComponent<CursorManager> ();
        cursorManager.cursorLocked = false;
        cursorManager.listening = false;

        PlayerController2 playerController = GetComponent<PlayerController2> ();
        playerController.shouldRotate = false;
        playerController.listening = false;

        popup.transform.SetParent (GameObject.Find("Canvas").transform);
        popup.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
        allowedToPickThingsUp = false;
        gameObject.GetComponent<UIManager> ().enabled = false;
        if (numPickedUp == dataSheetWrapper.Length) {
            Destroy (copy.gameObject);
        }
        if (numPickedUp == 0) {
            t.text = "Inventory full.";
        }
    }

    void disableLeftHand () {
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (leftHand[i] != null) {
                leftHand[i].GetComponent<Pickup>().active = false;
            }
        }
    }

    void disableRightHand () {
        for (int i = 0; i < NUM_SLOTS_PER_HAND; ++i) {
            if (rightHand[i] != null) {
                rightHand[i].GetComponent<Pickup>().active = false;
            }
        }
    }
}