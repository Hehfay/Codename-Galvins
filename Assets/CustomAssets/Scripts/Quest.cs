using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu ()]
public class Quest: ScriptableObject {

    public string questName;

    public QuestNode currentObjective;

    // Used for initialization at the editor level.
    public QuestNode firstobjective;

    public bool questComplete;

    Text questLogText;

    void AdvanceIfAble () {

        if (currentObjective.next.Length == 0) {
            questComplete = true;
            Debug.Log ("Quest Complete.");
            return;
        }

        switch (currentObjective.advanceConditionGate) {

            case AdvanceConditionGate.OR: {

            bool advance = false;

            foreach (KeyValuePair<int, bool> entry in currentObjective.advanceConditions) {
                advance = currentObjective.IsTaskComplete (entry.Key);
                if (advance) {
                    break;
                }
            }

            if (advance) {
                AdvanceQuestAndDisplayObjectives ();
            }

            } break;

            case AdvanceConditionGate.AND: {
                bool advance = true;

                foreach (KeyValuePair<int, bool> entry in currentObjective.advanceConditions) {
                    advance = currentObjective.IsTaskComplete (entry.Key);
                    if (!advance) {
                        break;
                    }
                }

                if (advance) {
                    AdvanceQuestAndDisplayObjectives ();
                }
                else {
                    currentObjective.ShowCurrentTasks ();
                }
                
            } break;

            default: {

            } break;

        }
    }

    public void ShowCurrentTasks () {
        currentObjective.ShowCurrentTasks ();
    }

    public void ProcessQuestTrigger (QuestTrigger questTrigger) {

        if (questComplete) {
            return;
        }

        if (questTrigger.questNode == currentObjective) {
            currentObjective.MarkTaskComplete (questTrigger.task);
            AdvanceIfAble ();
        }
        else {
            if (questTrigger.questNode.taskType != TaskType.TALK) {
                questTrigger.questNode.MarkTaskComplete (questTrigger.task);
            }
        }
    }

    public void ProcessQuestUnTrigger (QuestTrigger questTrigger) {

        if (questComplete) {
            return;
        }

        if (questTrigger.questNode == currentObjective) {
            currentObjective.MarkTaskIncomplete (questTrigger.task);
            currentObjective.ShowCurrentTasks ();
        }
        else {
            questTrigger.questNode.MarkTaskIncomplete (questTrigger.task);
            if (questTrigger.questNode == currentObjective.previous) {
                currentObjective = questTrigger.questNode;
                currentObjective.ShowCurrentTasks ();
            }
        }
    }

    public void SetCurrentObjectiveToFirstObjective () {
        currentObjective = firstobjective;
    }

    private void AdvanceQuestAndDisplayObjectives () {
        // TODO Select the next objective.
        currentObjective = currentObjective.next[0];
        currentObjective.ShowCurrentTasks ();
    }

    private void InitNode (QuestNode node) {
        node.advanceConditions = new Dictionary<int, bool>();
        node.taskDescription = new Dictionary<int, string> ();

        for (int j = 0; j < node.tasks.Length; ++j) {
            // Set every task to false.
            node.advanceConditions.Add (node.tasks[j], false);

            // Set the description of every task.
            node.taskDescription.Add (node.tasks[j], node.taskDescriptions[j]);
        }
        for (int i = 0; i < node.next.Length; ++i) {
            InitNode (node.next[i]);
        }
    }

    public void Init () {
        InitNode (firstobjective);
    }

    // TODO I don't like that this scriptable object interfaces with an outside gameobject but i can't really think
    // of a better way to send the current quest task to the canvas quest log.
    public void ShowCurrentTasksInLog () {
        GameObject.Find ("Sanity").GetComponentInChildren<Text>().text += questName + '\n';
        if (questComplete) {
            GameObject.Find ("Sanity").GetComponentInChildren<Text> ().text += " - Complete";
            return;
        }
        else {
            currentObjective.ShowCurrentTasksInLog ();
        }

    }
}
