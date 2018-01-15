using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.EventSystems;

public class OptionsTab: MonoBehaviour, IPointerClickHandler {

    public void OnPointerClick (PointerEventData eventData) {

        GameObject playerReference =
            transform.root.GetComponent<PlayerReferenceContainer> ().Player;

        if (playerReference == null) {
            Debug.Log ("playerReference is null.");
            return;
        }

        if (!(playerReference.GetComponent<NetworkIdentity> ().isLocalPlayer)) {
            Debug.Log ("Not the local player.");
            return;
        }
        else {
            Debug.Log ("This is the local player.");
        }

        if (UIState.uiState == UIState.UIStateEnum.Options) {
            return;
        }

        switch (UIState.uiState) {
            case UIState.UIStateEnum.Inventory:
            playerReference.GetComponent<UICharacterInventoryFactory> ().DestroyFactoryItem ();
            break;

            case UIState.UIStateEnum.QuestLog:
            playerReference.GetComponent<UIQuestLogFactory> ().DestroyFactoryItem ();
            break;
        }

        UIState.uiState = UIState.UIStateEnum.Options;
        playerReference.GetComponent<UIOptionsFactory> ().CreateFactoryItem ();
    }
}
