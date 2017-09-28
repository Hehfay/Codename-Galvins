using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Location : MonoBehaviour {

    public string locationName;
    private Collider collider;
    // Use this for initialization
	void Start () {
        collider = GetComponent<Collider> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnCollisionEnter(Collision collision) {
        // TODO: send the collision event to the actor
    }
}
