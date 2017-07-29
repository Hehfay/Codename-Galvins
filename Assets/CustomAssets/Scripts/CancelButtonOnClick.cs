using UnityEngine;
using UnityEngine.EventSystems;

public class CancelButtonOnClick: MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {
        SelectCountDone ();
    }

    // Send messages to say we are done.
    public static void SelectCountDone () {
        GameObject.Find ("InventoryController").GetComponent<InventoryController> ().EnableDragHandlers ();
        GameObject ButtonToDelete = GameObject.Find ("CountDisplay(Clone)");
        if (ButtonToDelete != null) {
            Destroy (ButtonToDelete);
        }
        UiClickHandler.countButtonEnabled = false;
    }
}
