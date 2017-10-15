using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public enum AdvanceConditionGate {
    AND,
    OR,
}

public enum TaskType {
    PICKUP,
    TALK,
}

[CreateAssetMenu()]
public class QuestNode : ScriptableObject {

    public Dictionary<int, bool> advanceConditions;
    public Dictionary<int, string> taskDescription;

    public string questLogDescription;

    public AdvanceConditionGate advanceConditionGate;
    public TaskType taskType;


    public QuestNode previous;
    public QuestNode[] next;

    // Since you can't have Dictionaries publicly editable in the editor, use these, then the dictionaries
    // will be created in code.
    public int[] tasks;
    public string[] taskDescriptions;

    // This will be read by an editor script to know which gameobjects to attach quest triggers to.
    public GameObject[] objectives;


    // Used if completing a quest gives you a reward.
    public GameObject[] rewards;
    public int[] rewardCount;

    public void ShowCurrentTasks () {
        for (int i = 0; i < tasks.Length; ++i) {
            string output;

            Debug.Assert (advanceConditions != null);
            Debug.Assert (taskDescription != null);

            taskDescription.TryGetValue (tasks[i], out output);

            bool taskComplete = false;

            advanceConditions.TryGetValue (tasks[i], out taskComplete);

            if (taskComplete) {
                output += " - Complete";
            }

            // Debug.Log (output);

        }
    }

    public void MarkTaskComplete (int index) {
        advanceConditions.Remove (index);
        advanceConditions.Add (index, true);
    }

    public void MarkTaskIncomplete (int index) {
        advanceConditions.Remove (index);
        advanceConditions.Add (index, false);
    }

    public bool IsTaskComplete (int task) {
        bool output = false;
        advanceConditions.TryGetValue (task, out output);
        return output;
    }

    public void ShowCurrentTasksInLog () {
        for (int i = 0; i < tasks.Length; ++i) {
            string output;

            Debug.Assert (advanceConditions != null);
            Debug.Assert (taskDescription != null);

            taskDescription.TryGetValue (tasks[i], out output);

            bool taskComplete = false;

            advanceConditions.TryGetValue (tasks[i], out taskComplete);

            if (taskComplete) {
                output += " - Complete";
            }
            GameObject.Find ("Sanity").GetComponentInChildren<Text>().text += "    > " + output + '\n';
        }
    }

}