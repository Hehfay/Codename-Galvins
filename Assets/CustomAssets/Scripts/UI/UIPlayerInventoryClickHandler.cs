using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using System;
using UnityEngine.UI;

// This script handles the click event for UI elements.
// When a UI element is clicked on, it is dropped.
public class UIPlayerInventoryClickHandler : MonoBehaviour, IPointerClickHandler {
    public static bool selectCountButtonCreated;

    public GameObject selectCountButton;
    private GameObject Canvas;

    void Start () {
        selectCountButtonCreated = false;
        Canvas = GameObject.Find ("Canvas");
    }

    public void OnPointerClick (PointerEventData eventData) {

        if (selectCountButtonCreated) {
            return;
        }

        Component comp = GetComponent<SlotObjectContainer>().obj.GetComponent(typeof(IObjectData));
        IObjectData objData = comp as IObjectData;

        if (objData.count() > 1) {
            selectCountButtonCreated = true;
            GameObject selectCountButtonReference = Instantiate (selectCountButton, Canvas.transform);
            selectCountButtonReference.GetComponent<RectTransform> ().localPosition = new Vector3 (0, 0, 0);
            selectCountButtonReference.GetComponentInChildren<Text> ().text = "1";
            selectCountButtonReference.GetComponentInChildren<PlusButtonOnClick> ().maxCount = objData.count ();
            selectCountButtonReference.SetActive (true);

            // Give the count button reference the gameobject and a reference to the character position.
            selectCountButtonReference.GetComponent<PlayerReferenceAndItemContainer>().Player =
                transform.root.GetComponent<PlayerReferenceContainer> ().Player;

            selectCountButtonReference.GetComponent<PlayerReferenceAndItemContainer> ().Item = GetComponent<SlotObjectContainer> ().obj;
            selectCountButtonReference.GetComponent<PlayerReferenceAndItemContainer> ().SlotObjectContainer = GetComponent<SlotObjectContainer> ();
            return;
        }

        DropItem();

        // TODO Wrap this nicely so other drop logic can use it.

        // See if the item has a quest trigger.
        QuestTriggerWrapper questTriggerWrapper = GetComponent<SlotObjectContainer> ().obj.GetComponent<QuestTriggerWrapper> ();
        if (questTriggerWrapper != null) {
            QuestTrigger pickupQuestTrigger = questTriggerWrapper.questTrigger;
            if (pickupQuestTrigger != null) {
                GameObject.Find("QuestManager").GetComponent<QuestManager> ().ProcessQuestUnTrigger(pickupQuestTrigger);
            }
        }
        gameObject.transform.SetParent (null);
        Destroy (gameObject);
    }

    public void DropItem () {
        GameObject player = transform.root.GetComponent<PlayerReferenceContainer> ().Player;
        GameObject dropItem = Instantiate(GetComponent<SlotObjectContainer> ().obj);
        GetComponent<SlotObjectContainer> ().obj.SetActive (true);
        dropItem.SetActive (true);
        dropItem.transform.position = player.transform.position;
        if (GetComponent<SlotObjectContainer>().obj.activeInHierarchy) {
            Destroy (GetComponent<SlotObjectContainer>().obj);
        }
        dropItem.transform.SetParent (null);
        Destroy (gameObject);
    }
}