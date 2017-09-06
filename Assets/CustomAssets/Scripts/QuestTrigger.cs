using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestTrigger : MonoBehaviour {
    public int nextObjective;

    // The objective that must be complete before advancing.
    public int advanceCondition;

    public Quest quest;
}
