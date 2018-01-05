using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInputHandler : MonoBehaviour {
    private bool active;
    private GameObject Canvas;
    public GameObject slotItemPrefab;

	void Start () {
        Canvas = GameObject.Find ("Canvas");
        active = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E)) {
            active = !active;

            if (!active) {
                // This is not good because this reads from the UI to the character 
                // inventory component. This should happen before this point
                // and all that should be needed is the foreach loop below.
                GetComponent<UICharacterInventoryFactory> ().DestroyFactoryItem ();

                InteractUtility.InteractEnd (gameObject);
            }

            foreach (Transform t in Canvas.transform) {
                Destroy (t.gameObject);
            }
            if (active) {

                InteractUtility.InteractStart (gameObject);

                GetComponent<UITabsFactory> ().CreateFactoryItem ();
                if (GetComponent<PlayerSettings>().rememberUIState) {
                    if (UIState.uiState == UIState.UIStateEnum.Inventory) {
                        GetComponent<UICharacterInventoryFactory> ().CreateFactoryItem (slotItemPrefab);
                    }
                    else if (UIState.uiState == UIState.UIStateEnum.QuestLog) {
                        GetComponent<UIQuestLogFactory> ().CreateFactoryItem ();
                    }
                    else if (UIState.uiState == UIState.UIStateEnum.Options) {
                        GetComponent<UIOptionsFactory> ().CreateFactoryItem ();
                    }
                }
                else {
                    GetComponent<UICharacterInventoryFactory> ().CreateFactoryItem (slotItemPrefab);
                    UIState.uiState = UIState.UIStateEnum.Inventory;
                }
            }
        }
	}
}