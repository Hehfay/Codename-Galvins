using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestWrapper : MonoBehaviour {
    public int currentObjective;
    public Quest quest;
    public Dictionary<int, bool> ObjectiveCompletionStatus;

    void Start () {
        currentObjective = quest.objectiveValue[0];
        ObjectiveCompletionStatus = new Dictionary<int, bool>();
        ObjectiveCompletionStatus.Add (0, true);
    }

    public void DisplayObjective () {
        string output;
        quest.ObjectiveDescription.TryGetValue (currentObjective, out output);
        Debug.Log (output);
    }

}