using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIInteraction : MonoBehaviour, IInteractable {

    public string characterName;

    public bool Interact (GameObject gameobject) {

        // TODO Checks to see if you can interact with the character.
        // For example, is this character attacking you? etc.

        // gameobject.GetComponent<QuestManager> ().ProcessQuestTrigger (GetComponent<QuestTrigger>());

        UIController uiController = GameObject.Find ("Canvas").GetComponent<UIController>();

        uiController.EnableDialog (gameobject, gameObject);
        /*
        uiController.DisplayName (characterName);
        AIDialogContainer localContainer = GetComponent<AIDialogContainer> ();

        uiController.CreateDialogTopicButtons (localContainer, );

        for (int i = GetComponent<AIDialogContainer>().dialogTopics.Length - 1; i >= 0; --i) {
            uiController.CreateDialogTopicButtons (localContainer.dialogTopics[i], i);
        }
        */
        return true;
    }

    public string ToolTip () {
        return characterName;
    }
}
