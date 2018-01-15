using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class InventoryTab: MonoBehaviour, IPointerClickHandler {

    public GameObject slotItemPrefab;

    public void OnPointerClick (PointerEventData eventData) {
        if (!transform.root.GetComponent<PlayerReferenceContainer>().Player.GetComponent<NetworkIdentity>().isLocalPlayer) {
            return;
        }

        if (UIState.uiState == UIState.UIStateEnum.Inventory) {
            return;
        }

        switch (UIState.uiState) {
            case UIState.UIStateEnum.QuestLog:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIQuestLogFactory> ().DestroyFactoryItem ();
            break;

            case UIState.UIStateEnum.Options:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIOptionsFactory> ().DestroyFactoryItem ();
            break;
        }

        UIState.uiState = UIState.UIStateEnum.Inventory;
        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().CreateFactoryItem (slotItemPrefab);
    }
}
