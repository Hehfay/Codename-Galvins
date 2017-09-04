using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManagerScript : MonoBehaviour {
    public List<QuestWrapper> ActiveQuests;

    public void Start () {
        ActiveQuests = new List<QuestWrapper> ();
    }

    public void ShowCurrentObjectives () {
        for (int i = 0; i < ActiveQuests.Count; ++i) {
            ActiveQuests[i].DisplayObjective ();
        }
    }
}