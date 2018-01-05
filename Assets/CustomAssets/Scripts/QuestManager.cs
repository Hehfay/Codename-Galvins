using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour {

    public List<Quest> quests;

    void Start () {
        if (false) {
            // If there are multiple quest managers in the scene that is bad.
            Debug.Log ("Start Quest Manager.");
        }
        for (int i = 0; i < quests.Count; ++i) {
            quests[i].Init ();
            quests[i].ShowCurrentTasks ();
        }
    }

    public void ProcessQuestTrigger (Quest quest, QuestTrigger trigger) {
        if (!quest.active) {
            quest.active = true;
            quest.SetCurrentObjectiveToFirstObjective ();
            quest.InitDialog ();
        }
        else {
            quest.ProcessQuestTrigger (trigger);
        }
    }

    public void QuestPostProcessing (Quest quest, QuestTrigger trigger) {
        quest.PostProcessing ();
    }

    public void ProcessQuestTrigger (QuestTrigger questTrigger) {
        Debug.Log("Process Quest Trigger");
        if (questTrigger == null) {
            return;
        }

        // TODO A better way to lookup the quests.
        for (int i = 0; i < quests.Count; ++i) {
            if (questTrigger.quest == quests[i]) {
                quests[i].ProcessQuestTrigger (questTrigger);
                break;
            }
        }
    }

    public void ProcessQuestUnTrigger (QuestTrigger questTrigger) {
        Debug.Log ("Processing Quest UnTrigger");
        // TODO A better way to lookup the quests.
        for (int i = 0; i < quests.Count; ++i) {
            if (questTrigger.quest == quests[i]) {
                quests[i].ProcessQuestUnTrigger (questTrigger);
                break;
            }
        }
    }

    public string getDialog (QuestTrigger questTrigger) {
        // TODO A better way to lookup the quests.
        for (int i = 0; i < quests.Count; ++i) {
            if (questTrigger.quest == quests[i]) {
                return quests[i].getDialog ();
            }
        }
        return "";
    }

    public void WriteToQuestLog () {
        for (int i = 0; i < quests.Count; ++i) {
            quests[i].ShowCurrentTasksInLog ();
        }
    }
}