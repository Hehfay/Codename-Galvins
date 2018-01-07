using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContainerFactory : MonoBehaviour {
    public GameObject Container;
    public GameObject ContainerSlot;
    public GameObject SlotItemPrefab;
    public GameObject PlayerReference;

    private GameObject Canvas;
    private List<GameObject> references;

    void Update () {
        if (Input.GetKeyDown(KeyCode.F)) {
            DestoryFactoryItem ();
        }
    }

    private void DestoryFactoryItem () {
        GetComponent<Container> ().PopulateContainerWithUIData (references[0]);

        InteractUtility.InteractEnd (PlayerReference);
        PlayerReference.GetComponent<PlayerMovementController> ().enabled = true;
        PlayerReference.GetComponent<UICharacterInventoryFactory> ().DestroyFactoryItem ();
        PlayerReference.GetComponent<UIInputHandler> ().enabled = true;
        PlayerReference.GetComponent<UITabsFactory> ().enabled = true;
        Destroy (references[0]);
        references.Clear ();
        enabled = false;
    }

    public void CreateFactoryItem (List<GameObject> inventory, GameObject playerReference) {
        PlayerReference = playerReference;
        if (references == null) {
            references = new List<GameObject>();
        }
        if (Canvas == null) {
            Canvas = GameObject.Find ("Canvas");
        }
        references.Add(Instantiate (Container, Canvas.transform, false));

        for (int i = 0; i < inventory.Count; ++i) {
            // Make a slot a child of the "Content".

            GameObject newSlot = Instantiate (ContainerSlot, references[0].transform.GetChild(0).transform.GetChild(0), false);

            // Make a slot item prefab a child of the slot that was just created.
            GameObject slotItem = Instantiate (SlotItemPrefab, newSlot.transform, false);

            Component component = inventory[i].GetComponent (typeof(IObjectData));
            IObjectData objectData = component as IObjectData;
            if (objectData.count() == 1) {
                slotItem.GetComponent<Text> ().text = objectData.objectName ();
            }
            else {
                slotItem.GetComponent<Text> ().text = objectData.objectName() + " x" + objectData.count();
            }
            slotItem.GetComponent<SlotObjectContainer> ().obj = inventory[i];
        }
        references[0].transform.root.GetComponent<ContainerInventoryReferenceContainer> ().ContainerInventory = GetComponent<Container> ();
    }
}