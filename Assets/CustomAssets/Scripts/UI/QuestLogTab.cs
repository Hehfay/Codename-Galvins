using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class QuestLogTab : MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {
        if (!transform.root.GetComponent<PlayerReferenceContainer>().Player.GetComponent<NetworkIdentity>().isLocalPlayer) {
            return;
        }

        if (UIState.uiState == UIState.UIStateEnum.QuestLog) {
            return;
        }

        switch (UIState.uiState) {
            case UIState.UIStateEnum.Inventory:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UICharacterInventoryFactory> ().DestroyFactoryItem ();
            break;

            case UIState.UIStateEnum.Options:
            transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIOptionsFactory> ().DestroyFactoryItem ();
            break;
        }

        UIState.uiState = UIState.UIStateEnum.QuestLog;
        transform.root.GetComponent<PlayerReferenceContainer> ().Player.GetComponent<UIQuestLogFactory> ().CreateFactoryItem ();
    }
}
