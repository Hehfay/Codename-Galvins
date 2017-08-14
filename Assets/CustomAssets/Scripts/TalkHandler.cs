using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkHandler : MonoBehaviour {

    public GameObject TalkPrompt;
    GameObject newTalkPrompt;

    public void DeletePrompt () {
        Destroy (newTalkPrompt);
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag != "Quest") return;

        if (!gameObject.GetComponent<Character>().alreadyInstantiated && 
            gameObject.GetComponent<Character>().allowedToPickThingsUp) {

            gameObject.GetComponent<Character>().alreadyInstantiated = true;
            newTalkPrompt = Instantiate (TalkPrompt) as GameObject;
            newTalkPrompt.transform.SetParent (GameObject.Find ("Canvas").transform);
            newTalkPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);

        }
        gameObject.GetComponent<Character>().currentCollider = other;
    }

    void OnTriggerExit (Collider other) {
        if (other.tag != "Quest") return;
        gameObject.GetComponent<Character>().alreadyInstantiated = false;
        gameObject.GetComponent<Character>().currentCollider = null;
        Destroy (newTalkPrompt);
    }

    void OnTriggerStay (Collider other) {
        if (other.tag != "Quest") return;
        if (!gameObject.GetComponent<Character>().alreadyInstantiated &&
            gameObject.GetComponent<Character>().allowedToPickThingsUp) {

            gameObject.GetComponent<Character>().alreadyInstantiated = true;
            newTalkPrompt = Instantiate (TalkPrompt) as GameObject;
            newTalkPrompt.transform.SetParent (GameObject.Find ("Canvas").transform);
            newTalkPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
        }
        if (gameObject.GetComponent<Character>().alreadyInstantiated &&
            !gameObject.GetComponent<Character>().allowedToPickThingsUp) {
            gameObject.GetComponent<Character>().alreadyInstantiated = false;
            Destroy (newTalkPrompt);
        }
        gameObject.GetComponent<Character>().currentCollider = other;
    }
}
