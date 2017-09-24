using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AIPackage : ScriptableObject {
    public Vector3 getTarget () {
        return GameObject.Find("PrancingPony").transform.position;
    }
}
