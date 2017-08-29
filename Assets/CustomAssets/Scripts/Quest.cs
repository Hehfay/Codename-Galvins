using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Quest : ScriptableObject {
    public Objective[] Objectives;

    // This should be set to zero unless you want to start at some other objective for debugging.
    public int currentObjectiveIndex;

    public Objective currentObjective () {
        return Objectives[currentObjectiveIndex];
    }

    public void AdvanceObjective () {
        currentObjectiveIndex++;
        if (currentObjectiveIndex > Objectives.Length) {
            currentObjectiveIndex = Objectives.Length - 1;
        }
    }
}
