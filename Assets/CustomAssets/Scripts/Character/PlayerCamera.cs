using UnityEngine;
using UnityEngine.Networking;


public class PlayerCamera : MonoBehaviour {

    public Transform playerTransform;

    // Use this for initialization
    private void Start() {

    }

	
	// Update is called once per frame
	void Update () {
	}

    public void setTarget (Transform target) {
        playerTransform = target;
        this.transform.parent = playerTransform;
        this.transform.localPosition = new Vector3(0f, 0.0f, 0f);
        this.transform.localRotation = new Quaternion(0f, 0f, 0f, 1f);
    }
}
