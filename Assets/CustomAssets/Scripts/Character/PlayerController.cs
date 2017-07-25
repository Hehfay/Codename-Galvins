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
    [Range(0.0f, 100.0f)]
    public float frictionCoeff = 2.5f;
    public float stickyCoeff = 0.5f;

    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float accelRate = 5.0f;
    public float strafeMultiplier = 0.5f; // accel multipliler for strafe direction
    public float walkSpeed = 1.4f;
    public float runSpeed = 4.0f;
    public float sprintSpeed = 10.0f;
    public float jumpSpeed = 3.0f; // average jump speed of human
    public float minSpeed = 0.1f; // average jump speed of human

    private float xInput;
    private float zInput;
    private float xRotInput;
    private float yRotInput;
    private bool jumpInput;
    private bool isWalking;
    private bool isSprinting;
    private CollisionFlags collisionFlags;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        // get the character controller of parent object
        characterController = GetComponent<CharacterController>();
        // setup character position
        RaycastHit hit; // raycast hit for below raycast
        Vector3 pos = this.transform.position;
        float height = characterController.height;
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit)) { // cast ray from current player position into ground beneath
            this.transform.position = new Vector3(pos.x, hit.point.y + (height / 2) + 0.2f, pos.z); // set the player to land on the ground beneath
        }
        isAlive = true;
        // setup camera
        accelRate += frictionCoeff;
        cameraObj = new GameObject();
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 0.525f, 0.2f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
    }
    void Update() {
        getInput(); // get the input only once per frame
    }
    public void FixedUpdate() {
        if (!isLocalPlayer) { // networking related: this makes only local player controlled by this script
            return;
        }
        // rotate model(horizontal)
        transform.Rotate(0f, yRotInput, 0f, Space.World);
        // rotate camera (vertical)
        // have to clamp rotation around xAxis (vertical look)
        RotateXAxisClampedBidirectionally(-xRotInput, 70);

        // do translational movement
        float speed = runSpeed; // determine max speed based on running/walking/sprinting
        if (isSprinting) { // sprinting overrides others
            speed = sprintSpeed;
        } else if (isWalking) {
            speed = walkSpeed;
        }

        Vector3 velocity = characterController.velocity; // set new velocity to current velocity for now
        Vector3 desiredAccelDirection = transform.forward * zInput * accelRate + transform.right * xInput * accelRate * strafeMultiplier; // get the desired accel direction
        Vector3 desiredAcceleration = Vector3.zero; // acceleration input by player ; zero if no collision with surface beneath
        Vector3 accelDirectionPlane; // this is the vertical plane that intersects the desired acceleration direction

        // Convert above desired direction into actual direction along survace beneath
        RaycastHit hitInfo;
        bool isOnGround = Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out hitInfo,
                           characterController.height / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (isOnGround) {
            accelDirectionPlane = Vector3.Cross(desiredAccelDirection, Vector3.up); // this is the plane that intersets both y axis and movement direction, always vertical
            desiredAccelDirection = Vector3.Cross(hitInfo.normal, accelDirectionPlane); // this is the line of intersection between the vertical plane and the plane walking on.
            desiredAccelDirection = desiredAccelDirection.normalized; // normalize the direction of the accel
            desiredAcceleration = desiredAccelDirection * accelRate; // now get actual acceleration by taking direction * accelRate
        }
        Vector3 frictionalAccel; // no friction unless grounded
        if (characterController.isGrounded) { // is on ground
            frictionalAccel = -velocity.normalized * frictionCoeff; // since we are on ground, we have friction
            if (jumpInput) { // character is on ground, so can jump
                velocity.y = jumpSpeed; // apply directly to velocity so that it happens explosively, and bc we only have one frame to do this
            } else { // apply sticky velocity so that the character will stick to the ground
                // now to do some interesting calculations about whether the player will be grounded next frame.
            }
        } else { // not on ground, so set friction to zero vector
            frictionalAccel = Vector3.zero; // no friction unless grounded
        }
        
        Vector3 velocityOnPlaneBeneathPlayer = Vector3.ProjectOnPlane(velocity, hitInfo.normal); // project the velocity onto surface beneath, this gives the velocity that is not caused due to sticky/gravity
        if ((frictionalAccel.magnitude * Time.fixedDeltaTime) > velocityOnPlaneBeneathPlayer.magnitude && desiredAcceleration.magnitude == 0) { // friction would actually cause velocity in negative direction
            frictionalAccel = -velocityOnPlaneBeneathPlayer / Time.fixedDeltaTime; // so make it so that velocity will be zero due to friction
        }
        if (velocity.magnitude > speed + Mathf.Epsilon) { // since going too fast, remove player input portion of acceleration, leave gravity
            desiredAcceleration = Vector3.zero;
        }
        Vector3 totalAccel = desiredAcceleration + frictionalAccel + Physics.gravity; // total accel
        velocity += totalAccel * Time.fixedDeltaTime; // add current velocity plus the acceleration * time
        if (velocityOnPlaneBeneathPlayer.magnitude < speed + Mathf.Epsilon && velocity.magnitude > speed - Mathf.Epsilon) {
            velocity = Vector3.ClampMagnitude(velocity, speed); // clamp at max speed value
        } else if (velocity.magnitude < minSpeed && desiredAcceleration == Vector3.zero) { // no input and speed < minSpeed => remove x and z components of velocity
            velocity = new Vector3(0.0f, velocity.y, 0.0f);
        }

        DebugOutput(totalAccel, velocity, frictionalAccel);

        RaycastHit nextHitInfo;
        bool groundedNextFrame = Physics.SphereCast(transform.position + velocity * Time.deltaTime, characterController.radius, Vector3.down, out nextHitInfo
                    , characterController.height / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (groundedNextFrame) {
            // since we are in this if, we think the character will be grounded next frame (think bc we did not take into account collisions, etc.),
            // so no sticky velocity to add
        } else { // since here, we think that the character will not be grounded next frame
            bool groundedNextFrameWithSticky = Physics.SphereCast(transform.position + (velocity + new Vector3(0.0f, -stickyCoeff, 0.0f)) * Time.deltaTime
                , characterController.radius, Vector3.down, out nextHitInfo
                    , characterController.height / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            if (!jumpInput && groundedNextFrameWithSticky) { // if not jumping, add the ground sticky velocity
                velocity.y = -stickyCoeff;
            }
        }
        collisionFlags = characterController.Move(velocity*Time.fixedDeltaTime);

        bool suicide = Input.GetKeyDown(KeyCode.K); // kill ; TODO: remove this, its just a dumb testing feature
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

    /**
     * This function gets the player input for this controller
     */
    void getInput() {
        xRotInput = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity; // get rotate input
        yRotInput = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity; // get rotate input
        xInput = Input.GetAxis("Horizontal"); // get input
        zInput = Input.GetAxis("Vertical"); // get input
        isSprinting = Input.GetButton("Sprint"); // is user trying to sprint
        isWalking = Input.GetButton("Walk"); // is user trying to walk
        jumpInput = Input.GetButton("Jump"); // is user trying to jump

    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (collisionFlags == CollisionFlags.Below)
        {
            return;
        }

        if (body == null || body.isKinematic)
        {
            return;
        }
        body.AddForceAtPosition(characterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }

    /**
     * Outputs Relevant Debug info
     */
    void DebugOutput(Vector3 accel, Vector3 velocity, Vector3 friction) {
        //Debug.Log("Accel: " + accel);
        Debug.Log("Velocity: " + velocity);
        //Debug.Log("Friction: " + friction);
    }
}