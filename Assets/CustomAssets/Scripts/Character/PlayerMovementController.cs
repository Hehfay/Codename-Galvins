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

    PlayerRotation playerRotation;
    PlayerTranslation playerTranslation;

    public override void OnStartLocalPlayer() {
        base.OnStartLocalPlayer();
        // get the character controller of parent object
        //characterController = GetComponent<CharacterController>();
        // setup character position
        RaycastHit hit; // raycast hit for below raycast
        Vector3 pos = this.transform.position;
        //height = characterController.height;
        //radius = characterController.radius;
        if (Physics.Raycast(new Ray(pos, Vector3.down), out hit)) { // cast ray from current player position into ground beneath
            this.transform.position = new Vector3(pos.x, hit.point.y /*+ (height / 2) + 0.2f */, pos.z); // set the player to land on the ground beneath
        }
        //jumpState = JumpState.Grounded;
        //jumpCoeff = Mathf.Sqrt(-1 * Physics.gravity.y * jumpHeight * 2); // velocity required to reach jumpHeight
        isAlive = true;
        shouldRotate = true;
        // setup camera
        //accelRate += frictionCoeff;
        /*cameraObj = new GameObject();
        cameraObj.transform.parent = this.transform;
        cameraObj.transform.localPosition = new Vector3(0f, 1.545f, 0f);
        cameraObj.transform.rotation = new Quaternion(0f, 0f, 0f, 1f);
        Camera.main.GetComponent<PlayerCamera>().setTarget(cameraObj.transform);
        camera = cameraObj.GetComponentInChildren<Camera>();
        camera.fieldOfView = 60f; */
        playerRotation = GetComponent<PlayerRotation>();
        playerTranslation = GetComponent<PlayerTranslation>();

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
            playerRotation.Rotate(new Vector2(xRot, yRot));
            // rotate model(horizontal)
            //transform.Rotate(0f, yRotInput, 0f, Space.World);
            // rotate camera (vertical)
            // have to clamp rotation around xAxis (vertical look)
            //RotateXAxisClampedBidirectionally(-xRotInput, maxVerticalLookAngle);
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
        PlayerTranslation.RunState runState = PlayerTranslation.RunState.Running;
        if (isSprinting) { // sprinting overrides others
            runState = PlayerTranslation.RunState.Sprinting;
        }
        else if (isWalking) {
            runState = PlayerTranslation.RunState.Walking;
        } else if (zInput == 0 && xInput == 0) { // not moving
            runState = PlayerTranslation.RunState.Still;
        }
        playerTranslation.SetRunState(runState);
        playerTranslation.SetMovementDirection(new Vector2(xInput, zInput));
        if (jumpInput) {
            playerTranslation.Jump();
        }

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
        Vector3 velocity = playerTranslation.GetCurrentVelocity();
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

    public void OnCollisionEnter(Collider other) {
        if (!isServer) {
            return;
        } else {

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
