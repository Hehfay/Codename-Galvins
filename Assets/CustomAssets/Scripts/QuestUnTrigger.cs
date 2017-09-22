using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// If the current objective is not 'thisObjective' then there will be no change in the current objective.
// thisObjective will simply be marked as imcomplete.
[CreateAssetMenu()]
public class QuestUnTrigger : ScriptableObject {
    // This is the objective that will be marked as incomplete during untrigger.
    public int thisObjective;

    // If the current objective is thisObjective, then the new current 
    // objective will be previousObjective;
    public int previousObjective;

    public Quest quest;
}