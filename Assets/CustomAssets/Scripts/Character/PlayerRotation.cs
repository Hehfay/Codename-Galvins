using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour, ActorRotationInterface {

    [Tooltip("Determines behavior of vertical input. If true, can only look up and down so far before hitting invisible wall.")]
    public bool xRotationClamped = true;
    [Tooltip("Only used when X Rotation Clamped is true.")]
    public float xPositiveRotationClampDegree;
    [Tooltip("Only used when X Rotation Clamped is true.")]
    public float xNegativeRotationClampDegree;

    private float xLookSensitivity;
    private float yLookSensitivity;
    private GameObject cameraObj;

    // Use this for initialization
    void Start () {
        cameraObj = new GameObject("Camera Transform Parent");
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 1.545f, 0f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
        Camera camera = cameraObj.GetComponentInChildren<Camera>();
        camera.fieldOfView = 60f;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Rotate (Vector2 xyRot) {
        if (this.enabled) { // if not enabled, do nothing
            transform.Rotate(0f, xyRot.y, 0f, Space.World);
            // rotate camera (vertical)
            // have to clamp rotation around xAxis (vertical look)
            RotateXAxisClampedBidirectionally(-xyRot.x);
        }
    }

    private void RotateXAxisClampedBidirectionally(float angle) {


        // convert the incoming angle as degrees to rads
        float rad = MathUtil.convertDegreeToRad(angle);
        // get current angles
        Quaternion curRot = cameraObj.transform.localRotation;
        float curRad = Mathf.Asin(curRot.x) * 2;
        float curAngle = MathUtil.convertRadToDegree(curRad);

        float desiredAngle = curAngle + angle;
        if (desiredAngle < 0) {
            desiredAngle = Mathf.Max(desiredAngle, -xNegativeRotationClampDegree);
        }
        else {
            desiredAngle = Mathf.Min(desiredAngle, xPositiveRotationClampDegree);
        }
        float desiredRad = MathUtil.convertDegreeToRad(desiredAngle);

        float cosFinalAngle = 0;
        float sinFinalAngle = 0;
        cosFinalAngle = Mathf.Cos(desiredRad / 2);
        sinFinalAngle = Mathf.Sin(desiredRad / 2);
        Quaternion rotation = new Quaternion(sinFinalAngle, 0f, 0f, cosFinalAngle);
        cameraObj.transform.localRotation = rotation;
        Transform cameraObjTrans = cameraObj.transform;
    }
}
