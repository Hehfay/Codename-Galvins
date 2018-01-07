using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovementController : NetworkBehaviour {

    // local variables
    private Animator animator; // player animator
    bool isAlive; // is this character alive?
    public GameObject cameraObj; // this is where the player camera will be held
    Camera camera; // the camera that is made child of cameraObj
    private CharacterController characterController; // character controller of parent object
    float radius; // how wide is player
    float height; // how tall is player
    [SerializeField]
    private float gravityMultiplier; // how much gravity affects player. Can raise or lower based on desired feel of the game (or special effects)
    [Range(0.0f, 100.0f)]
    public float frictionCoeff = 2.5f; // how much does friction slow player down
    public float xSensitivity = 1.0f; // sensitivity settings
    public float ySensitivity = 1.0f; // sensitivity settings
    public float maxVerticalLookAngle = 50.0f;

    public float accelRate = 4.0f; // how fast player accelerates
    public float airbornAccelRate = 2.0f; // how fast player accelerates when in air
    public float strafeMultiplier = 0.75f; // accel multipliler for strafe direction (you dont strafe as fast as you move forward
    public float walkSpeed = 1.4f; // how fast player moves while walking
    public float runSpeed = 3.0f; // how fast player moves while running
    public float sprintSpeed = 6.0f; // how fast player moves while sprinting
    public float jumpSpeed = 3.0f; // run speed while jumping
    public float jumpHeight = 1.5f; // average jump height of human
    public float jumpLandDuration = 0.1f; // how long in the landing phase
    public float minSpeed = 0.1f; // average jump speed of human
    

    private float xInput; // left-right move input
    private float zInput; // forward-backward move input
    private float xRotInput; // mouse x input
    private float yRotInput; // mouse y input
    private bool leftMouseClicked; // did player click left mb
    private bool rightMouseClicked; // did player click right mb
    private Vector2 mousePos; // mouse input
    private bool jumpInput; // player pressed jump button
    private bool isWalking; // player trying to walk
    private bool isSprinting; // player trying to sprint
    private bool wasSprinting; // for FOV kick, whenever I get around to implementing it (if we want)
    private bool isGrounded; // is on ground this fixedUpdate
    private bool wasGrounded; // was on ground last fixedUpdate
    private CollisionFlags collisionFlags; // not used; here bc it was in StandardAssets PlayerController; may be useful in future.

    private JumpState jumpState;
    private float jumpCoeff;
    private float timeLanded; // when did the player land
    private float airTime; // how long been in air

    public bool shouldRotate;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        // get the character controller of parent object
        characterController = GetComponent<CharacterController>();
        // setup character position
        RaycastHit hit; // raycast hit for below raycast
        Vector3 pos = this.transform.position;
        height = characterController.height;
        radius = characterController.radius;
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit)) { // cast ray from current player position into ground beneath
            this.transform.position = new Vector3(pos.x, hit.point.y /*+ (height / 2) + 0.2f */, pos.z); // set the player to land on the ground beneath
        }
        jumpState = JumpState.Grounded;
        jumpCoeff = Mathf.Sqrt(-1 * Physics.gravity.y * jumpHeight * 2); // velocity required to reach jumpHeight
        isAlive = true;
        shouldRotate = true;
        // setup camera
        accelRate += frictionCoeff;
        cameraObj = new GameObject();
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 1.545f, 0f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
        camera = cameraObj.GetComponentInChildren<Camera>();
        camera.fieldOfView = 60f;

        animator = GetComponent<Animator>();

        // disable mesh renders as they get in way of view frustum
        SkinnedMeshRenderer[] meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach(SkinnedMeshRenderer r in meshRenderers) {
            r.enabled = false;
        }
    }
    
    public void FixedUpdate() {
        if (!isLocalPlayer) { // networking related: this makes only local player controlled by this script
            return;
        }
    }

    public void LateUpdate() {
        // rotate camera(vertical) and model(horizontal)
        float xRot = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity; // get rotate input
        float yRot = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity; // get rotate input
        if (shouldRotate) {
            // rotate model(horizontal)
            transform.Rotate(0f, yRotInput, 0f, Space.World);
            // rotate camera (vertical)
            // have to clamp rotation around xAxis (vertical look)
            RotateXAxisClampedBidirectionally(-xRotInput, maxVerticalLookAngle);
        }
    }

    public void Update() {
        if (!isLocalPlayer) { // networking related: this makes only local player controlled by this script
            return;
        }
        getInput(); // get the input only once per frame
        
        // process click
        if (leftMouseClicked) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(ray, out hit, 2.0f);
            Collider collider = hit.collider;
            if (collider != null) {
                ActorAI npc = collider.GetComponent<ActorAI>();
                if (npc != null) {
                    npc.doInteract();
                }
            }
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
        Vector3 desiredAccelDirection;
        if (speed > velocity.magnitude) {
            desiredAccelDirection = transform.forward * zInput + transform.right * xInput * strafeMultiplier; // get the desired accel direction
        } else {
            desiredAccelDirection = new Vector3(0f, 0f, 0f); // No acceleration since moving faster than desired / allowed
        }
        Vector3 desiredAcceleration = Vector3.zero; // acceleration input by player ; zero if no collision with surface beneath
        Vector3 accelDirectionPlane; // this is the vertical plane that intersects the desired acceleration direction
        Vector3 velocityDirectionPlane = Vector3.Cross(velocity, Vector3.up); // this is the vertical plane that intersects current velocity

        // Convert above desired direction into actual direction along survace beneath
        RaycastHit groundHitInfo;
        RaycastHit headHitInfo;
        bool isOnGround = //characterController.isGrounded;
        Physics.SphereCast(transform.position + new Vector3(0.0f, radius, 0.0f), radius, Vector3.down, out groundHitInfo,
                           0.2f /*height / 2 - radius*/, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        bool hitHead = Physics.SphereCast(transform.position, radius, Vector3.up, out headHitInfo,
                           height /*/ 2 */ - radius, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        if (jumpInput && jumpState == JumpState.Grounded) {
            jumpState = JumpState.Inititated;
            Vector2 xzPlaneSpeed = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), jumpSpeed); // clamp speed along xz plane
            velocity = new Vector3(xzPlaneSpeed.x, jumpCoeff, xzPlaneSpeed.y); // jumping
            
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
                desiredAcceleration += Physics.gravity;
                desiredAcceleration += (transform.forward * zInput + transform.right * xInput * strafeMultiplier) * airbornAccelRate;
                Vector2 xzPlaneVelocity = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), jumpSpeed);
                velocity = new Vector3(xzPlaneVelocity.x, velocity.y, xzPlaneVelocity.y) + (desiredAcceleration * Time.deltaTime);
            }
            // TODO: play land noise
        }
        else if (!isOnGround) {
            // TODO: figure out why this happens
            // Debug.Log("I am not on the ground, somehow.");
            jumpState = JumpState.Airborn;
        }
        else if (jumpState == JumpState.JustLanded) {
            if (Time.time - timeLanded > jumpLandDuration) {
                jumpState = JumpState.Grounded;
                timeLanded = 0.0f; // sentinel value indicating not in use
            }
        }
        if (jumpState == JumpState.Grounded || jumpState == JumpState.JustLanded) {
            Vector3 groundHitNormal = groundHitInfo.normal.normalized;
            float angleFromVertical = MathUtil.convertRadToDegree(Mathf.Acos(Vector3.Dot(groundHitNormal, Vector3.up)));
            velocityDirectionPlane = Vector3.Cross(velocity, Vector3.up);
            Vector3 velocityDirection = Vector3.Cross(groundHitInfo.normal, velocityDirectionPlane).normalized; // line of intersection between vertical plane of velocity and plane walking on.
            if (angleFromVertical > characterController.slopeLimit) { // slide down the terrain bc its too steep
                velocityDirectionPlane = Vector3.Cross(groundHitNormal, Vector3.up);
                velocityDirection = Vector3.Cross(groundHitNormal, velocityDirectionPlane).normalized;
                velocity = velocityDirection * runSpeed;
            }
            else {
                accelDirectionPlane = Vector3.Cross(desiredAccelDirection, Vector3.up); // this is the plane that intersets both y axis and movement direction, always vertical
                desiredAccelDirection = Vector3.Cross(groundHitInfo.normal, accelDirectionPlane).normalized; // this is the line of intersection between the vertical plane and the plane walking on.
                desiredAccelDirection = desiredAccelDirection.normalized; // normalize the direction of the accel
                desiredAcceleration = desiredAccelDirection * accelRate; // now get actual acceleration by taking direction * accelRate
            }

            Vector3 friction = velocityDirection * frictionCoeff * -1;
            if ((friction.magnitude * Time.fixedDeltaTime) > velocity.magnitude && desiredAcceleration.magnitude == 0) { // friction would actually cause velocity in negative direction
                friction = -velocity / Time.fixedDeltaTime; // so make it so that velocity will be zero due to friction
            }
            velocity = velocity.magnitude * velocityDirection + (desiredAcceleration + friction) * Time.deltaTime;
            if (velocity.magnitude > sprintSpeed) { // can't go faster than max velocity
                velocity = Vector3.ClampMagnitude(velocity, sprintSpeed);
            }
        } else {
        }
        Vector3 moveVal = velocity * Time.deltaTime;

        collisionFlags = characterController.Move(moveVal);
        // now check if that move causes problems

        bool suicide = Input.GetKeyDown(KeyCode.K); // kill ; TODO: remove this, its just a dumb testing feature

        switch (Input.inputString) //get keyboard input, probably not a good idea to use strings here...Garbage collection problems with regards to local string usage are known to happen
        {                          //the garbage collection memory problem arises from local alloction of memory, and not freeing it up efficiently
            case "p":
                animator.SetTrigger("Pain");//the animator controller will detect the trigger pain and play the pain animation
                break;
            /*case "a":
                animator.SetInteger("Death", 1);//the animator controller will detect death=1 and play DeathA
                break;
            case "b":
                animator.SetInteger("Death", 2);//the animator controller will detect death=2 and play DeathB
                break;
            case "c":
                animator.SetInteger("Death", 3);//the animator controller will detect death=3 and play DeathC
                break; */
            case "n":
                animator.SetBool("NonCombat", true);//the animator controller will detect this non combat bool, and go into a non combat state "in" this weaponstate
                break;
            default:
                break;
        }
        if (velocity.magnitude > 0.1f) {
            animator.SetBool("Idling", false);
        }
        else {
            animator.SetBool("Idling", true);
        }

        if (leftMouseClicked) {
            animator.SetTrigger("Use");
        }
    }

    /**
     * This function handles look rotations about the local x-axis (vertical looking).
     * It only changes the camera, not the player so that the model does not lean
     * in weird ways. It allows a clamp angle be passed in so that the the camera
     * does not rotate beyond this angle from rest angle in either direction.
     */
    void RotateXAxisClampedBidirectionally(float angle, float clampAngle) {
        

        // convert the incoming angle as degrees to rads
        float rad = MathUtil.convertDegreeToRad(angle);
        // get current angles
        Quaternion curRot = cameraObj.transform.localRotation;
        float curRad = Mathf.Asin(curRot.x) * 2;
        float curAngle = MathUtil.convertRadToDegree(curRad);

        float desiredAngle = curAngle + angle;
        if (desiredAngle < 0) {
            desiredAngle = Mathf.Max(desiredAngle, -clampAngle);
        }
        else {
            desiredAngle = Mathf.Min(desiredAngle, clampAngle);
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

    /**
     * This function gets the player input for this controller
     */
    void getInput() {
        xRotInput = Input.GetAxis("Mouse Y") * Time.deltaTime * xSensitivity; // get rotate input
        yRotInput = Input.GetAxis("Mouse X") * Time.deltaTime * ySensitivity; // get rotate input
        xInput = Input.GetAxis("Horizontal"); // get input
        zInput = Input.GetAxis("Vertical"); // get input
        leftMouseClicked = Input.GetMouseButtonDown(0);
        mousePos = Input.mousePosition;
        isSprinting = Input.GetButton ("Sprint"); // is user trying to sprint
        isWalking = Input.GetButton ("Walk"); // is user trying to walk
        jumpInput = Input.GetButton ("Jump"); // is user trying to jump
        shouldRotate = (Input.GetKeyDown (KeyCode.E) ^ shouldRotate); // Exclusive OR
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
