using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class InitializeQuests : EditorWindow {
    [MenuItem ("Quest/Initialize Quests")]

    public static void ShowWindow () {
        EditorWindow.GetWindow (typeof(InitializeQuests));
    }

    void OnGUI () {
        if (GUILayout.Button("Initialize Quests")) {

            Quest[] quests = Resources.FindObjectsOfTypeAll<Quest> ();

            for (int i = 0; i < quests.Length; ++i) {
                quests[i].questComplete = false;
                quests[i].questStarted  = false;
                quests[i].SetCurrentObjectiveToFirstObjective ();
            }
            Debug.Log ("Done initializing.");
        }
    }

    private void InitNode (QuestNode q) {
        q.advanceConditions = new Dictionary<int, bool>();
        q.taskDescription = new Dictionary<int, string> ();
        for (int j = 0; j < q.tasks.Length; ++j) {
            // Set every task to false.
            q.advanceConditions.Add (q.tasks[j], false);

            // Set the description of every task.
            q.taskDescription.Add (q.tasks[j], q.taskDescriptions[j]);
        }
        for (int k = 0; k < q.next.Length; ++k) {
            InitNode (q.next[k]);
        }
    }
}