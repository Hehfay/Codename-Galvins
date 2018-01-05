using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropButtonOnClick: MonoBehaviour, IPointerClickHandler {
    // I don't like this much because i am copying and pasting code...

    // TODO Process Quest Untriggers.

    public void OnPointerClick (PointerEventData eventData) {

        int value = Convert.ToInt32(transform.parent.GetComponentInChildren<Text> ().text);

        GameObject player = transform.parent.GetComponent<PlayerReferenceAndItemContainer> ().Player;
        GameObject item = transform.parent.GetComponent<PlayerReferenceAndItemContainer> ().Item;
        SlotObjectContainer slotObjectContainer = transform.parent.GetComponent<PlayerReferenceAndItemContainer> ().SlotObjectContainer;

        Component comp = item.GetComponent(typeof(IObjectData));
        IObjectData objData = comp as IObjectData;

        objData.decreaseCount (value);

        if (objData.count () == 0) {
            GameObject dropItem = Instantiate (item);
            dropItem.transform.position = player.transform.position;
            dropItem.transform.SetParent (null);
            dropItem.SetActive (true);

            Component component = dropItem.GetComponent (typeof(IObjectData));
            IObjectData objectData = component as IObjectData;
            objectData.setCount (value);

            Destroy (slotObjectContainer.gameObject);
            Destroy (item);
        }
        else {
            GameObject dropItem = Instantiate (item);
            Component dropComp = dropItem.GetComponent(typeof(IObjectData));
            IObjectData dropObjData = dropComp as IObjectData;
            if (dropObjData.count() == 1) {
                slotObjectContainer.gameObject.GetComponent<Text> ().text = dropObjData.objectName ();
            }
            else {
                slotObjectContainer.gameObject.GetComponent<Text> ().text = dropObjData.objectName() + " x" + dropObjData.count();
            }
            dropObjData.setCount (value);
            dropItem.transform.position = player.transform.position;
            dropItem.transform.SetParent (null);
            dropItem.SetActive (true);
        }
        UIPlayerInventoryClickHandler.selectCountButtonCreated = false;
        Destroy (transform.parent.gameObject);
    }
}