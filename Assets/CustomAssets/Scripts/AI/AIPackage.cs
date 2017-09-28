using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AIPackage : ScriptableObject {
    public Vector3 getTarget () {
        //return GameObject.Find("marker_PrancingPony").transform.position;
        return new Vector3(Random.Range(10.0f, 450.0f), Random.Range(1.0f, 2.0f), Random.Range(10.0f, 450.0f));
    }
}
