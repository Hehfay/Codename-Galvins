using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CreateQuestTriggers : EditorWindow {
    /*
    [MenuItem ("Quest/Create Quest Triggers")]

    public static void ShowWindow () {
        EditorWindow.GetWindow (typeof(CreateQuestTriggers));
    }

    void OnGUI () {
        if (GUILayout.Button ("Create Quest Triggers")) {

            Quest[] quests = Resources.FindObjectsOfTypeAll<Quest> ();

            for (int i = 0; i < quests.Length; ++i) {

                Debug.Log ("Attaching triggers for the quest " + quests[i]);

                AttachTrigger (quests[i], quests[i].currentObjective);

            }
            Debug.Log ("Done.");
        }
    }

    private void AttachTrigger (Quest quest, QuestNode node) {
        Debug.Log ("Attaching triggers for the quest " + quest + " and node " + node);
        for (int j = 0; j < node.objectives.Length; ++j) {

            QuestTrigger qt = node.objectives[j].GetComponent<QuestTrigger> ();
            if (qt != null) {
                Debug.Log (qt.gameObject.ToString() + " already has a quest trigger attached to it. Skipping.");
                continue;
            }

            node.objectives[j].AddComponent<QuestTrigger> ();
            node.objectives[j].GetComponent<QuestTrigger> ().questNode = node;
            node.objectives[j].GetComponent<QuestTrigger> ().quest = quest;
            node.objectives[j].GetComponent<QuestTrigger> ().task = node.tasks[j];
        }

        for (int k = 0; k < node.next.Length; ++k) {
            AttachTrigger (quest, node.next[k]);
        }

    }
    */
}