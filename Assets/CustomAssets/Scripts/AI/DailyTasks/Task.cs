using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task {

    public string taskName;
    public float timeBegin; // what time of day this task begins
    public float timeEnd; // what time of day this task ends
    public int[] days; // which days this task applies to
    public Transform taskLocation;
    public int doWhat; // useless for the moment; datatype will change once the data type is defined
}
