using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestWrapper : MonoBehaviour {
    public int currentObjective;
    public Quest quest;
    public Dictionary<int, bool> ObjectiveCompletionStatus;
    public bool isActiveQuest;

    public int currentObjectiveIndex;

    void Start () {
        currentObjective = quest.objectiveValue[0];
        ObjectiveCompletionStatus = new Dictionary<int, bool>();
        ObjectiveCompletionStatus.Add (0, true);
        isActiveQuest = false;
        currentObjectiveIndex = 0;
        GameObject.Find ("QuestManager").GetComponent<QuestManagerScript> ().ActiveQuests.Add (this);
    }

    public void DisplayObjective () {
        string output;
        quest.ObjectiveDescription.TryGetValue (currentObjective, out output);
        Debug.Log (output);
    }

    public void NextObjective () {
        currentObjectiveIndex += 1;
        if (currentObjectiveIndex > (quest.objectiveValue.Length - 1)) {
            currentObjectiveIndex = quest.objectiveValue.Length - 1;
        }
        currentObjective = quest.objectiveValue[currentObjectiveIndex];
    }
}