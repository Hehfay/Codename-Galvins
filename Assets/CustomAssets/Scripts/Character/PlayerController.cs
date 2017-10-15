using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : NetworkBehaviour {
    // local variables
    bool isAlive;
    public GameObject cameraObj; // this is where the player camera will be held
    private CharacterController characterController; // character controller of parent object
    [SerializeField]
    private float gravityMultiplier;
    [Range(0.0f, 100.0f)]
    public float frictionCoeff = 2.5f;

    public float xSensitivity = 1.0f;
    public float ySensitivity = 1.0f;

    public float accelRate = 5.0f;
    public float strafeMultiplier = 0.5f; // accel multipliler for strafe direction
    public float walkSpeed = 1.4f;
    public float runSpeed = 4.0f;
    public float sprintSpeed = 10.0f;
    public float jumpSpeed = 3.0f; // run speed while jumping
    public float jumpHeight = 1.5f; // average jump height of human
    public float jumpLandDuration = 0.1f; // how long in the landing phase
    public float minSpeed = 0.1f; // average jump speed of human
    

    private float xInput;
    private float zInput;
    private float xRotInput;
    private float yRotInput;
    private bool jumpInput;
    private bool isWalking;
    private bool isSprinting;
    private bool wasSprinting; // for FOV kick, whenever I get around to implementing it (if we want)
    private bool isGrounded;
    private bool wasGrounded;
    private CollisionFlags collisionFlags;

    private JumpState jumpState;
    private float jumpCoeff;
    private float timeLanded;

    public bool shouldRotate;
    public bool listening;

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
        jumpState = JumpState.Grounded;
        jumpCoeff = Mathf.Sqrt(-1 * Physics.gravity.y * jumpHeight * 2);
        isAlive = true;
        shouldRotate = true;
        // setup camera
        accelRate += frictionCoeff;
        cameraObj = new GameObject();
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 0.525f, 0.2f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
    }
    
    public void FixedUpdate() {
        if (!isLocalPlayer) { // networking related: this makes only local player controlled by this script
            return;
        }
        getInput(); // get the input only once per frame
        // rotate camera(vertical) and model(horizontal)
        float xRot = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity; // get rotate input
        float yRot = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity; // get rotate input
        if (shouldRotate) {
            // rotate model(horizontal)
            transform.Rotate(0f, yRotInput, 0f, Space.World);
            // rotate camera (vertical)
            // have to clamp rotation around xAxis (vertical look)
            RotateXAxisClampedBidirectionally(-xRotInput, 70);
        }


        // do translational movement
        float speed = runSpeed; // determine max speed based on running/walking/sprinting
        if (isSprinting) { // sprinting overrides others
            speed = sprintSpeed;
        }
        else if (isWalking) {
            speed = walkSpeed;
        }

        Vector3 velocity = characterController.velocity; // set new velocity to current velocity for now
        Vector3 desiredAccelDirection = transform.forward * zInput * accelRate + transform.right * xInput * accelRate * strafeMultiplier; // get the desired accel direction
        Vector3 desiredAcceleration = Vector3.zero; // acceleration input by player ; zero if no collision with surface beneath
        Vector3 accelDirectionPlane; // this is the vertical plane that intersects the desired acceleration direction
        Vector3 velocityDirectionPlane = Vector3.Cross(velocity, Vector3.up); // this is the vertical plane that intersects current velocity

        // Convert above desired direction into actual direction along survace beneath
        RaycastHit groundHitInfo;
        RaycastHit headHitInfo;
        bool isOnGround = Physics.SphereCast(transform.position, characterController.radius, Vector3.down, out groundHitInfo,
                           characterController.height / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        bool hitHead = Physics.SphereCast(transform.position, characterController.radius, Vector3.up, out headHitInfo,
                           characterController.height / 2, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (jumpInput && jumpState == JumpState.Grounded) {
            jumpState = JumpState.Inititated;
            velocity = new Vector3(velocity.x, jumpCoeff, velocity.z); // jumping
            // TODO: play jump noise
        }
        else if (jumpState == JumpState.Inititated && hitHead) {
            jumpState = JumpState.Grounded; // could also choose just landed and play noises for both leaving ground and landing; or neither
        }
        else if (jumpState == JumpState.Inititated && !isOnGround) { // just left the ground
            jumpState = JumpState.Airborn;
        }
        else if (jumpState == JumpState.Airborn) {
            if (isOnGround) {
                jumpState = JumpState.JustLanded;
                timeLanded = Time.time; // timestamp of when landed
            } else {
                velocity += Physics.gravity * Time.fixedDeltaTime;
            }
            // TODO: play land noise
        }
        else if (jumpState == JumpState.JustLanded) {
            if (Time.time - timeLanded > jumpLandDuration) {
                jumpState = JumpState.Grounded;
                timeLanded = 0.0f; // sentinel value indicating not in use
            }
        }
        if (jumpState == JumpState.Grounded || jumpState == JumpState.JustLanded) {
            accelDirectionPlane = Vector3.Cross(desiredAccelDirection, Vector3.up); // this is the plane that intersets both y axis and movement direction, always vertical
            velocityDirectionPlane = Vector3.Cross(velocity, Vector3.up);
            desiredAccelDirection = Vector3.Cross(groundHitInfo.normal, accelDirectionPlane).normalized; // this is the line of intersection between the vertical plane and the plane walking on.
            Vector3 velocityDirection = Vector3.Cross(groundHitInfo.normal, velocityDirectionPlane).normalized; // line of intersection between vertical plane of velocity and plane walking on.
            desiredAccelDirection = desiredAccelDirection.normalized; // normalize the direction of the accel
            desiredAcceleration = desiredAccelDirection * accelRate; // now get actual acceleration by taking direction * accelRate

            Vector3 friction = velocityDirection * frictionCoeff * -1;
            if ((friction.magnitude * Time.fixedDeltaTime) > velocity.magnitude && desiredAcceleration.magnitude == 0) { // friction would actually cause velocity in negative direction
                friction = -velocity / Time.fixedDeltaTime; // so make it so that velocity will be zero due to friction
            }
            //Vector3 totalAccel = desiredAcceleration + friction + Physics.gravity;
            //velocity = velocity.magnitude * velocityDirection + totalAccel * Time.fixedDeltaTime;
            //velocity = Vector3.ClampMagnitude(velocity, speed);
            velocity = velocity.magnitude * velocityDirection + (desiredAcceleration + friction) * Time.fixedDeltaTime;
            velocity = Vector3.ClampMagnitude(velocity, speed);
            velocity += Physics.gravity * Time.fixedDeltaTime;
        } else {
        }

        collisionFlags = characterController.Move(velocity * Time.fixedDeltaTime);

        bool suicide = Input.GetKeyDown(KeyCode.K); // kill ; TODO: remove this, its just a dumb testing feature
    }

    /**
     * This function handles look rotations about the x-axis (vertical looking).
     * It only changes the camera, not the player so that the model does not lean
     * in weird ways. It allows a clamp angle be passed in so that the the camera
     * does not rotate beyond this angle from rest angle in either direction.
     */
    void RotateXAxisClampedBidirectionally(float angle, float clampAngle) {
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
        }
        else {
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


    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (collisionFlags == CollisionFlags.Below) {
            return;
        }

        if (body == null || body.isKinematic) {
            return;
        }
        body.AddForceAtPosition(characterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    public enum JumpState {
        Inititated
        , Airborn
        , JustLanded
        , Grounded
    };
}