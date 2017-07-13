using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : NetworkBehaviour {
    // local variables
    bool isAlive;
    public GameObject cameraObj; // this is where the player camera will be held
    private CharacterController characterController; // character controller of parent object

    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float walkSpeed = 1.0f;
    public float runSpeed = 2.0f;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        // get the character controller of parent object
        characterController = GetComponent<CharacterController>();
        // setup character position
        RaycastHit hit; // raycast hit for below raycast
        Vector3 pos = this.transform.position;
        float height = characterController.height;
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit)) { // cast ray from current player position into ground beneath
            this.transform.position = new Vector3(pos.x, hit.point.y + (height / 2), pos.z); // set the player to land on the ground beneath
        }
        isAlive = true;
        // setup camera
        cameraObj = new GameObject();
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 0.525f, 0f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
    }
    void Update() {
    }
    public void FixedUpdate() {
        if (!isLocalPlayer) {
            return;
        }
        if (isAlive) {
            float x = Input.GetAxis("Horizontal") * Time.deltaTime * 3.0f;
            float z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;
            bool isRunning = Input.GetButton("Run");
            float speed = isRunning ? runSpeed : walkSpeed;

            float xRot = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity;
            float yRot = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity;

            Vector3 desiredMove = transform.forward * z + transform.right * x;

            // have to clamp rotation around xAxis (vertical look)
            transform.Rotate(0f, yRot, 0f, Space.World);
            Debug.Log("Current local rotation: " + transform.localRotation.ToString());
            RotateXAxisClampedBidirectionally(-xRot, 70);
            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                               characterController.height, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized; // only project if there was a hit
            }
            Vector3 move = desiredMove * speed;
            // TODO: add speed tracking to eliminate slowness down slopes
            // TODO: track collisions from character controller
            // TODO: add in gravity from Physics
            characterController.Move(move*Time.fixedDeltaTime);

            bool suicide = Input.GetKeyDown(KeyCode.K);
            if (suicide) {
                isAlive = false;
            }
        }
    }

    /**
     * This function handles look rotations about the x-axis (vertical looking).
     * It only changes the camera, not the player so that the model does not lean
     * in weird ways. It allows a clamp angle be passed in so that the the camera
     * does not rotate beyond this angle from rest angle in either direction.
     */
    void RotateXAxisClampedBidirectionally (float angle, float clampAngle) {
        // conversion constants
        float degreeToRadConst = Mathf.PI / 180;
        float radToDegreeConst = 180 / Mathf.PI;

        // convert the incoming angle as degrees to rads
        float rad = angle * degreeToRadConst;
        // get current angles
        Quaternion curRot = cameraObj.transform.localRotation;
        float curRad = Mathf.Asin(curRot.x) * 2;
        float curAngle = curRad * radToDegreeConst;

        float desiredAngle = curAngle + angle;
        if (desiredAngle < 0) {
            desiredAngle = Mathf.Max(desiredAngle, -clampAngle);
        } else {
            desiredAngle = Mathf.Min(desiredAngle, clampAngle);
        }
        float desiredRad = desiredAngle * degreeToRadConst;

        float cosFinalAngle = 0;
        float sinFinalAngle = 0;
        cosFinalAngle = Mathf.Cos(desiredRad / 2);
        sinFinalAngle = Mathf.Sin(desiredRad / 2);
        Quaternion rotation = new Quaternion(sinFinalAngle, 0f, 0f, cosFinalAngle);
        cameraObj.transform.localRotation = rotation;
    }
}