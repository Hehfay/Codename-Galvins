using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestWrapper : MonoBehaviour {
    public int currentObjective;
    public Quest quest;

    void Start () {
        currentObjective = quest.objectiveValue[0];
    }

    public void DisplayObjective () {
        string output;
        quest.ObjectiveDescription.TryGetValue (currentObjective, out output);
        Debug.Log (output);
    }
}