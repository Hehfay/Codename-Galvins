using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestHandler : MonoBehaviour {

    public GameObject TalkPrompt;
    GameObject newTalkPrompt;

    void OnTriggerEnter (Collider other) {
        if (other.CompareTag ("Quest")) {
            if (!gameObject.GetComponent<Character>().alreadyInstantiated && 
                gameObject.GetComponent<Character>().allowedToPickThingsUp) {

                gameObject.GetComponent<Character>().alreadyInstantiated = true;
                newTalkPrompt = Instantiate (TalkPrompt) as GameObject;
                newTalkPrompt.transform.SetParent (GameObject.Find ("Canvas").transform);
                newTalkPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);

            }
            gameObject.GetComponent<Character>().currentCollider = other;
        }
    }

    public void DeletePrompt () {
        Destroy (newTalkPrompt);
    }

    void OnTriggerStay () {
    }
}
