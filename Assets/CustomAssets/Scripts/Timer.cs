using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour {

    public float duration;

    void Update () {
        duration -= Time.deltaTime;
        if (duration < 0) {
            Destroy (this);
        }
    }

    void OnDestroy () {
        transform.parent.GetComponent<UIController> ().DestoryWhatWasPickedUp ();
    }
}
