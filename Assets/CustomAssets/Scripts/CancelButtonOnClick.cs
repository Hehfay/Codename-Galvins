using UnityEngine;
using UnityEngine.EventSystems;

public class CancelButtonOnClick: MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {
        SelectCountDone ();
    }

    // We are done with the select count popup so send messages to go back into 
    // inventory just opened by player state.
    public static void SelectCountDone () {
        GameObject.Find ("Player(Clone)").GetComponent<InventoryController> ().EnableDragHandlers ();
        GameObject ButtonToDelete = GameObject.Find ("CountDisplay(Clone)");
        if (ButtonToDelete != null) {
            Destroy (ButtonToDelete);
        }
        UiClickHandler.countButtonEnabled = false;
    }
}
