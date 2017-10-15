using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteractableMonobehaviour : MonoBehaviour {

    // interact function
    public virtual void Interact() { }
}