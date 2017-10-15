using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The purpose of this class is to spawn the canvas when the player spawns.
public class GameobjectSpawner : MonoBehaviour {

    public GameObject canvas;

    // TODO Get rid of the object spawner.
    void Start () {
        Instantiate (canvas);
    }
}
