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

    public QuestWrapper getQuest (Quest q) {
        for (int i = 0; i < ActiveQuests.Count; ++i) {
            if (q == ActiveQuests[i].quest) {
                return ActiveQuests[i];
            }
        }
        // Should not happen; the QuestManager MUST have every quest.
        return null;
    }
}