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

    public bool active;
    public bool complete;

    Text questLogText;

    public void InitDialog () {
        dialog = currentObjective.dialog;
    }

    public bool canIAdvance () {
        if (!active || complete) {
            return false;
        }

        if (currentObjective.advanceConditionGate == AdvanceConditionGate.OR) {
            bool advance = false;
            foreach (KeyValuePair<int, bool> entry in currentObjective.advanceConditions) {
                advance = currentObjective.IsTaskComplete (entry.Key);
                if (advance) {
                    break;
                }
            }
            return advance;
        }

        if (currentObjective.advanceConditionGate == AdvanceConditionGate.AND) {
            bool advance = true;
            foreach (KeyValuePair<int, bool> entry in currentObjective.advanceConditions) {
                advance = currentObjective.IsTaskComplete (entry.Key);
                if (!advance) {
                    break;
                }
            }
            return advance;
        }
        Debug.Log ("The current objective does not have an advance condition gate!");
        Debug.Assert (false);
        return false;
    }

    public void ShowCurrentTasks () {
        currentObjective.ShowCurrentTasks ();
    }

    public void ProcessQuestTrigger (QuestTrigger questTrigger) {
        if (complete) {
            return;
        }

        if (questTrigger.questNode == currentObjective) {
            currentObjective.MarkTaskComplete (questTrigger.task);
        }
        else {
            if (questTrigger.questNode.taskType != TaskType.TALK) {
                questTrigger.questNode.MarkTaskComplete (questTrigger.task);
            }
        }

        while (canIAdvance()) {
            AdvanceQuestAndDisplayObjectives ();
            dialog = currentObjective.dialog;
        }
    }

    public void ProcessQuestUnTrigger (QuestTrigger questTrigger) {
        if (complete) {
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
                // When you go to the previous node, make the quest giver say
                // the repeated dialog.
                dialog = currentObjective.repeatedDialog;
            }
        }
    }

    public void SetCurrentObjectiveToFirstObjective () {
        currentObjective = firstobjective;
    }

    private void AdvanceQuestAndDisplayObjectives () {
        // TODO Select the next objective.
        givePlayersQuestNodeRewards ();
        if (currentObjective.next.Length == 0) {
            complete = true;
        }
        else {
            currentObjective = currentObjective.next[0];
            currentObjective.ShowCurrentTasks ();
        }
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

    public void PostProcessing () {
        while (canIAdvance()) {
            AdvanceQuestAndDisplayObjectives ();
            dialog = currentObjective.dialog;
        }
    }

    // TODO I don't like that this scriptable object interfaces with an outside gameobject but i can't really think
    // of a better way to send the current quest task to the canvas quest log.
    public void ShowCurrentTasksInLog () {
        if (!active) {
            return;
        }
        GameObject.Find ("UIQuestLog(Clone)").transform.GetChild(0).GetComponentInChildren<Text>().text += questName + '\n';
        if (complete) {
            GameObject.Find ("UIQuestLog(Clone)").transform.GetChild(0).GetComponentInChildren<Text> ().text += " - Complete";
            return;
        }
        else {
            currentObjective.ShowCurrentTasksInLog ();
        }
    }

    private void givePlayersQuestNodeRewards () {
        // TODO Figure out how to remove GameObject.Find.
        for (int i = 0; i < currentObjective.rewards.Length; ++i) {

            GameObject questReward = Instantiate (currentObjective.rewards[i]);

            Component component = questReward.GetComponent (typeof(IObjectData));
            IObjectData objdata = component as IObjectData;
            objdata.setCount(currentObjective.rewardCount[i]);

            // if the interaction failed, drop the item at the player's feet.
            if (!questReward.GetComponent<PickupObject> ().Interact (GameObject.Find ("A03(Clone)"))) {
                questReward.transform.position = GameObject.Find ("A03(Clone)").transform.position;
            }
        }
    }
}