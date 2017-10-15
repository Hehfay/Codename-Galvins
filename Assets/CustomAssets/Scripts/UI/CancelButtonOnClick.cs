using UnityEngine;
using UnityEngine.EventSystems;

public class CancelButtonOnClick: MonoBehaviour, IPointerClickHandler {
    UIController uiController;

    void Start () {
        uiController = transform.parent.transform.parent.GetComponent<UIController>();
    }

    public void OnPointerClick (PointerEventData eventData) {
        SelectCountDone ();
    }

    // We are done with the select count popup so send messages to go back into 
    // inventory just opened by player state.
    public void SelectCountDone () {
        // The inventory controller is on the canvas.
        transform.parent.transform.parent.GetComponent<InventoryController> ().EnableDragHandlers ();
        uiController.DestoryCountButton ();
    }
}
