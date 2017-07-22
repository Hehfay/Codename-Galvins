using UnityEngine;
using UnityEngine.Networking;

[RequireComponent (typeof(CharacterController))]
public class PlayerController : NetworkBehaviour {
    // local variables
    bool isAlive;
    public GameObject cameraObj; // this is where the player camera will be held
    private CharacterController characterController; // character controller of parent object
    [SerializeField]
    private float gravityMultiplier;
    [Range(0.0f, 10.0f)]
    public float frictionCoeff = 2.5f;

    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float accelRate = 5.0f;
    public float strafeMultiplier = 0.5f; // accel multipliler for strafe direction
    public float walkSpeed = 1.4f;
    public float runSpeed = 4.0f;
    public float sprintSpeed = 10.0f;
    public float jumpSpeed = 3.0f; // average jump speed of human

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
        accelRate += frictionCoeff;
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
            // rotate camera(vertical) and model(horizontal)
            float xRot = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity; // get rotate input
            float yRot = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity; // get rotate input
            transform.Rotate(0f, yRot, 0f, Space.World);
            // have to clamp rotation around xAxis (vertical look)
            RotateXAxisClampedBidirectionally(-xRot, 70);

            // do translational movement
            float x = Input.GetAxis("Horizontal"); // get input
            float z = Input.GetAxis("Vertical"); // get input
            bool isSprinting = Input.GetButton("Sprint"); // is user trying to sprint
            bool isWalking = Input.GetButton("Walk"); // is user trying to walk
            float speed = runSpeed; // determine max speed based on running/walking/sprinting
            if (isSprinting) { // sprinting overrides others
                speed = sprintSpeed;
            } else if (isWalking) {
                speed = walkSpeed;
            }

            Vector3 velocity = characterController.velocity; // set new velocity to current velocity for now
            Vector3 desiredAccelDirection = transform.forward * z * accelRate + transform.right * x * accelRate * strafeMultiplier; // get the desired accel direction
            // clamp above so that diagonal directions are not faster than forward direction

            RaycastHit hitInfo;
            if (Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                               characterController.height, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                Vector3 accelDirectionPlane = Vector3.Cross(desiredAccelDirection, Vector3.up); // this is the plane that intersets both y axis and movement direction, always vertical
                desiredAccelDirection = Vector3.Cross(hitInfo.normal, accelDirectionPlane); // this is the line of intersection between the vertical plane and the plane walking on.
                desiredAccelDirection = desiredAccelDirection.normalized;
                desiredAccelDirection *= accelRate;
            }
            Vector3 frictionalAccel = -velocity.normalized * frictionCoeff;
            if (characterController.isGrounded) { // is on ground
                if (!characterController.velocity.Equals(Vector3.zero) && desiredAccelDirection.Equals(Vector3.zero)) { // character is moving but does not wish to be, so apply friction
                    //frictionAccel = frictionCoeff * Physics.gravity.magnitude;
                }
                bool jump = Input.GetButton("Jump"); // the jump button
                if (jump) { // character is on ground, so can jump
                    desiredAccelDirection += new Vector3(0.0f, jumpSpeed / Time.fixedDeltaTime, 0.0f); // divide by fixed delta bc the Move function is being given accels, not velocity, but for jump,
                    // since we only get one frame, we want it to be veloc
                }
            } else { // not on ground, so apply gravity
                desiredAccelDirection += Physics.gravity; // add gravity to the acceleration
            }
            if (velocity.magnitude > speed + Mathf.Epsilon) { // since going to fast, remove the part of this 
                desiredAccelDirection -= Vector3.Project(desiredAccelDirection, velocity);
            }
            velocity = characterController.velocity + (desiredAccelDirection + frictionalAccel) * Time.fixedDeltaTime; // add current velocity plus the acceleration * time
            if (velocity.magnitude < speed + Mathf.Epsilon || velocity.magnitude > speed - Mathf.Epsilon) {
                velocity = Vector3.ClampMagnitude(velocity, speed); // clamp at max speed value
            }
            Debug.Log("Velocity: " + velocity.magnitude);

            //velocity = Vector3.ClampMagnitude(velocity, speed); // clamp at max speed value

            // TODO: add speed tracking to eliminate slowness down slopes
            // TODO: track collisions from character controller
            // TODO: add in gravity from Physics
            characterController.Move(velocity*Time.fixedDeltaTime);

            bool suicide = Input.GetKeyDown(KeyCode.K); // kill ; TODO: remove this, its just a dumb testing feature
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