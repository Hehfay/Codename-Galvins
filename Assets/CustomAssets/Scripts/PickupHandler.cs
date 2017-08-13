using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHandler : MonoBehaviour {

    public GameObject PickupTextPrompt;
    GameObject newPickupTextPrompt;

    public void DeletePrompt () {
        Destroy (newPickupTextPrompt);
    }

    void OnTriggerEnter (Collider other) {
        if (other.tag != "Pickup") return;

        if (!gameObject.GetComponent<Character>().alreadyInstantiated &&
            gameObject.GetComponent<Character>().allowedToPickThingsUp) {

            gameObject.GetComponent<Character>().alreadyInstantiated = true;
            newPickupTextPrompt = Instantiate (PickupTextPrompt) as GameObject;
            newPickupTextPrompt.transform.SetParent (GameObject.Find ("Canvas").transform);
            newPickupTextPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);

        }
        gameObject.GetComponent<Character>().currentCollider = other;
    }
    void OnTriggerStay (Collider other) {
        if (other.tag != "Pickup") return;

        if (!gameObject.GetComponent<Character>().alreadyInstantiated &&
            gameObject.GetComponent<Character>().allowedToPickThingsUp) {

            gameObject.GetComponent<Character>().alreadyInstantiated = true;
            newPickupTextPrompt = Instantiate (PickupTextPrompt) as GameObject;
            newPickupTextPrompt.transform.SetParent (GameObject.Find ("Canvas").transform);
            newPickupTextPrompt.GetComponent<RectTransform> ().localPosition = new Vector3 (0, -50, 0);
        }
        if (gameObject.GetComponent<Character>().alreadyInstantiated &&
            !gameObject.GetComponent<Character>().allowedToPickThingsUp) {
            gameObject.GetComponent<Character>().alreadyInstantiated = false;
            Destroy (newPickupTextPrompt);
        }
        gameObject.GetComponent<Character>().currentCollider = other;
    }

    void OnTriggerExit (Collider other) {
        if (other.tag != "Pickup") return;
        gameObject.GetComponent<Character>().alreadyInstantiated = false;
        gameObject.GetComponent<Character>().currentCollider = null;
        Destroy (newPickupTextPrompt);
    }
}
