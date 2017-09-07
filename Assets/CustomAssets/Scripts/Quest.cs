using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Quest : ScriptableObject {

    public Dictionary<int, string> ObjectiveDescription;

    public string[] objectiveDescription;
    public int[] objectiveValue;

    public int firstObjective;

    // Scriptable objects do NOT have 'void Start ()'.
    void OnEnable () {
        ObjectiveDescription = new Dictionary<int, string>();

        firstObjective = objectiveValue[0];
        Debug.Assert (objectiveDescription.Length == objectiveValue.Length);
        for (int i = 0; i < objectiveValue.Length; ++i) {
            ObjectiveDescription.Add (objectiveValue[i], objectiveDescription[i]);
        }
    }
}

