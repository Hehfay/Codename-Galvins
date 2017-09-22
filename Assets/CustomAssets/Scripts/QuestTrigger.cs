using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class QuestTrigger : ScriptableObject {
    public int nextObjective;
    public int thisObjective;

    // The objective that must be complete before advancing.
    // This int looks up the completion status in the Quest Wrapper completion status map.
    // 0 means there is no condition.
    public int advanceCondition;

    public Quest quest;
}
