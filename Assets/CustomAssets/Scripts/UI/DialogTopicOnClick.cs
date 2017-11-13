using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogTopicOnClick : MonoBehaviour, IPointerClickHandler {
    public string dialogTopic;
    public string dialog;
    public QuestTrigger questTrigger;
    public QuestManager questManager;

    public void OnPointerClick (PointerEventData eventData) {

        if (questTrigger != null) {
            GameObject.Find ("QuestManager").GetComponent<QuestManager> ().StartQuestIfNotAlreadyStarted (questTrigger.quest);
            GameObject.Find ("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger (questTrigger);
            dialog = GameObject.Find ("QuestManager").GetComponent<QuestManager> ().getDialog (questTrigger);
        }

        // Tell the UIController to display the dialog.
        dialog = dialogTopic + "\n" + dialog;
        GetComponentInParent<UIController> ().DialogOptionClicked (dialog);
    }
}
