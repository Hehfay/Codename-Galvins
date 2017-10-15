using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManager : MonoBehaviour {

    public List<Quest> quests;

    void Start () {
        for (int i = 0; i < quests.Count; ++i) {
            quests[i].Init ();
            quests[i].ShowCurrentTasks ();
        }
    }

    public void ProcessQuestTrigger (QuestTrigger questTrigger) {
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
        // TODO A better way to lookup the quests.
        for (int i = 0; i < quests.Count; ++i) {
            if (questTrigger.quest == quests[i]) {
                quests[i].ProcessQuestUnTrigger (questTrigger);
                break;
            }
        }
    }

    public void WriteToQuestLog () {
        for (int i = 0; i < quests.Count; ++i) {
            quests[i].ShowCurrentTasksInLog ();
        }
    }
}