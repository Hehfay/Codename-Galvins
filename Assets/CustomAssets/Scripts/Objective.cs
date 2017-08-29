using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QuestType {
    Fetch,
    Talk,
};

[CreateAssetMenu()]
public class Objective : ScriptableObject {
    public QuestType questType;

    public PickupData[] FetchItems;
           public int[] FetchCount;

    public GameObject[] PeopleToTalkTo;
          public bool[] TalkedTo;

    // The game telling the player what to do.
    public string description;

    /*
    void Start () {
        Debug.Assert (FetchCount.Length == FetchItems.Length);
        Debug.Assert (PeopleToTalkTo.Length == TalkedTo.Length);
    }
    */

}
