using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterInventoryFactory: MonoBehaviour {
    public GameObject UICharacterInventory;

    private List<GameObject> references;
    private GameObject Canvas;

    void Start () {
        references = new List<GameObject> ();
        Canvas = GameObject.Find ("Canvas");
    }

    void Update () {
        if (references.Count != 0) {
            if (references[0] == null) {
                Debug.Log ("references[0] == null");
            }
        }
    }

    public void CreateFactoryItem (GameObject slotItemPrefab) {
        references.Clear ();
        references.Add (Instantiate (UICharacterInventory, Canvas.transform, false));
        references[0].GetComponent<UIInventoryPopulator> ().DisplayCharacterInventory (GetComponent<CharacterInventory> (), slotItemPrefab);
    }

    public void DestroyFactoryItem () {
        if (references.Count == 0) {
            return;
        }

        CharacterInventory characterInventory = GetComponent<CharacterInventory> ();

        references[0].GetComponent<UIInventoryPopulator> ().PopulateCharacterInventory (ref characterInventory);
        Destroy (references[0]);
        references.Clear ();
    }

    public void RefreshInventoryUI (GameObject slotItemPrefab) {
        references[0].GetComponent<UIInventoryPopulator> ().DisplayCharacterInventory (GetComponent<CharacterInventory> (), slotItemPrefab);
    }

    public void RefreshCharacterInventory () {
        CharacterInventory characterInventory = GetComponent<CharacterInventory> ();
        references[0].GetComponent<UIInventoryPopulator> ().PopulateCharacterInventory (ref characterInventory);
    }
}