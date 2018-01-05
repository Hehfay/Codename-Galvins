using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogTopicOnClick : MonoBehaviour, IPointerClickHandler {
    public string dialogTopic;
    public string dialog;
    public QuestTrigger questTrigger;
    public GameObject UIDialogText;

    public void OnPointerClick (PointerEventData eventData) {

        GameObject uiDialogText = Instantiate (UIDialogText,
                                               transform.parent.transform.parent.transform.parent.transform.parent.GetChild(0).GetChild(0).GetChild(0),
                                               false);

        string outputDialog = dialog;

        if (questTrigger != null) {
            GameObject.Find ("QuestManager").GetComponent<QuestManager> ().ProcessQuestTrigger (questTrigger.quest, questTrigger);

            outputDialog = GameObject.Find ("QuestManager").GetComponent<QuestManager> ().getDialog (questTrigger);

            // Hmm...
            GameObject.Find ("QuestManager").GetComponent<QuestManager> ().QuestPostProcessing (questTrigger.quest, questTrigger);
        }

        outputDialog = dialogTopic + "\n" + outputDialog;

        // Write the dialog to the Dialog box.
        uiDialogText.GetComponentInChildren<Text> ().text = outputDialog;
    }
}
