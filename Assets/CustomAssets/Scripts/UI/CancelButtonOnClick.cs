using UnityEngine;
using UnityEngine.EventSystems;

public class CancelButtonOnClick: MonoBehaviour, IPointerClickHandler {
    public void OnPointerClick (PointerEventData eventData) {
        UIPlayerInventoryClickHandler.selectCountButtonCreated = false;
        Destroy (transform.parent.gameObject);
    }
}