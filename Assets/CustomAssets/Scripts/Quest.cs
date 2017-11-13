using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu ()]
public class Quest: ScriptableObject {

    private string dialog;

    public string questName;

    public QuestNode currentObjective;

    // Used for initialization at the editor level.
    public QuestNode firstobjective;

    public bool questComplete;
    public bool questStarted;

    Text questLogText;

    public void InitDialog () {
        dialog = currentObjective.dialog;
    }

    void AdvanceIfAble () {

        if (currentObjective.next.Length == 0) {
            questComplete = true;
            Debug.Log ("Quest Complete.");

            // Drop the reward on the current node.
            givePlayersQuestNodeRewards ();

            dialog = currentObjective.dialog;

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

        // Instantiate the reward for this node.
        givePlayersQuestNodeRewards ();

        currentObjective = currentObjective.next[0];
        currentObjective.ShowCurrentTasks ();
        dialog = currentObjective.dialog;
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

    public string getDialog () {
        string returnDialog = dialog;
        dialog = currentObjective.repeatedDialog;
        return returnDialog;
    }

    public void Init () {
        InitNode (firstobjective);
    }

    // TODO I don't like that this scriptable object interfaces with an outside gameobject but i can't really think
    // of a better way to send the current quest task to the canvas quest log.
    public void ShowCurrentTasksInLog () {

        if (!questStarted) {
            return;
        }

        GameObject.Find ("Sanity").GetComponentInChildren<Text>().text += questName + '\n';
        if (questComplete) {
            GameObject.Find ("Sanity").GetComponentInChildren<Text> ().text += " - Complete";
            return;
        }
        else {
            currentObjective.ShowCurrentTasksInLog ();
        }
    }

    private void givePlayersQuestNodeRewards () {
        // TODO Figure out how to remove GameObject.Find.
        for (int i = 0; i < currentObjective.rewards.Length; ++i) {
            currentObjective.rewards[i].GetComponent<PickupItem> ().count = currentObjective.rewardCount[i];
            Debug.Log ("Dropping loot!");
            // if the interaction failed, drop the item at the player's feet.
            if (!currentObjective.rewards[i].GetComponent<PickupItem> ().Interact (GameObject.Find ("Player(Clone)"))) {
                Instantiate (currentObjective.rewards[i]).transform.position = GameObject.Find ("Player(Clone)").transform.position;
            }
        }
    }
}