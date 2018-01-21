using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(CharacterController))]
public class PlayerTranslation : NetworkBehaviour, ActorTranslationInterface {
    [Header("Acceleration fields")]
    [Tooltip("Rate of acceleration of player (m/s/s).")]
    public float accelRate = 65.0f; // how fast player accelerates
    [Tooltip("Multiplier applied to airborne acceleration.")]
    public float airbornAccelRate = 25.0f;
    [Header("Speed fields")]
    [Tooltip("Player speed when walking.")]
    public float walkSpeed = 1.4f; // how fast player moves while walking
    [Tooltip("Player speed when running.")]
    public float runSpeed = 4.0f; // how fast player moves while running
    [Tooltip("Player speed when sprinting. Maximum speed of player")]
    public float sprintSpeed = 7.0f; // how fast player moves while sprinting
    [Tooltip("Desired speed when sprinting. Maximum speed of player")]
    public float slideDownSlopeSpeed = 8.0f;
    [Tooltip("Maximum speed this player can move along xz plane when in air.")]
    public float maxXZAirborneSpeed = 3.0f;
    [Tooltip("Multiplier applied to strafe running speed.")]
    [Range(0.0f, 1.0f)]
    public float strafeMultiplier = 0.8f; // speed multipliler for strafe direction (you dont strafe as fast as you move forward
    [Tooltip("Multiplier applied to backpedal running speed.")]
    [Range(0.0f, 1.0f)]
    public float backpedalMultiplier = 0.5f;
    [Header("Jump fields")]
    [Tooltip("How high the player jumps.")]
    public float jumpHeight = 1.2f;
    [Tooltip("How long the player cannot jump after landing from previous jump.")]
    public float jumpLandDuration = 0.2f;
    [Header("Friction fields")]
    [Tooltip("Coefficient for applying friction to player.")]
    [Range(0.0f, 100.0f)]
    public float frictionCoefficient = 2.5f; // how much does friction slow player down

    CharacterController characterController;
    JumpState previousJumpState;
    JumpState currentJumpState;
    CrouchState currentCrouchState;
    CrouchState previousCrouchState;
    RunState currentRunState;
    RunState previousRunState;
    bool currentlyIsStrafing;
    bool previouslyWasStrafing;
    RelativeDirection strafeDirection;
    float timeLanded; // when did the player land; necessary to track jump state after landing

    private CollisionFlags collisionFlags; // not used; here bc it was in StandardAssets PlayerController; may be useful in future.

    float height;
    float radius;

    float jumpCoefficient; // coefficient that converts the height that the player jumps into an acceleration
    float zMovement;
    float xMovement;


    // Use this for initialization
    public override void OnStartLocalPlayer() {
        accelRate += frictionCoefficient; // do this so that the friction doesn't cancel out the given acceleration rate
        characterController = GetComponent<CharacterController>();
        height = characterController.height;
        radius = characterController.radius;

        previousJumpState = JumpState.Grounded;
        currentJumpState = JumpState.Grounded;
        previousCrouchState = CrouchState.Standing;
        currentCrouchState = CrouchState.Standing;
        previousRunState = RunState.Still;
        currentRunState = RunState.Still;
        previouslyWasStrafing = false;
        currentlyIsStrafing = false;

        jumpCoefficient = Mathf.Sqrt(-1 * Physics.gravity.y * jumpHeight * 2); // velocity required to reach jumpHeight
    }
	
	// Update is called once per frame
	void Update () {
        // get every frame in case they change (crouch,etc)
        height = characterController.height;
        radius = characterController.radius;
        // do translational movement
        Vector3 currentVelocity = characterController.velocity; // set new velocity to current velocity for now
        float desiredSpeed = runSpeed; // determine max speed based on running/walking/sprinting
        if (currentRunState == RunState.Sprinting) { // sprinting overrides others
            desiredSpeed = sprintSpeed;
        }
        else if (currentRunState == RunState.Walking) {
            desiredSpeed = walkSpeed;
        }
        if (zMovement < 0.0f) { // backpedaling, so can't spring and multiply by the backpedal multiplier
            if (currentRunState == RunState.Sprinting) {
                currentRunState = RunState.Running;
                desiredSpeed = runSpeed;
            }
            if (currentRunState == RunState.Running) {
                desiredSpeed *= backpedalMultiplier;
            }
        } else if (Mathf.Abs(xMovement) > 0.1f) { // xInput means trying to strafe, so sprinting gets demoted to running and speed is multiplied by strafe multiplier
            if (currentRunState == RunState.Sprinting) { // can sprint since not attempting to strafe
                currentRunState = RunState.Running;
                desiredSpeed = runSpeed;
            }
            if (currentRunState == RunState.Running) { // gotta put on the strafe multipler if running, but not if walking
                desiredSpeed *= strafeMultiplier;
            }
        }

        Vector3 desiredAccelerationDirection;
        if (desiredSpeed > currentVelocity.magnitude) {
            desiredAccelerationDirection = transform.forward * zMovement + transform.right * xMovement; // get the desired accel direction
        }
        else {
            desiredAccelerationDirection = new Vector3(0f, 0f, 0f); // No acceleration since moving faster than desired / allowed
        }
        

        //Vector3 velocity = characterController.velocity; // set new velocity to current velocity for now


        Vector3 desiredAcceleration = Vector3.zero; // acceleration input by player ; zero if no collision with surface beneath
        Vector3 accelDirectionPlane; // this is the vertical plane that intersects the desired acceleration direction
        Vector3 velocityDirectionPlane = Vector3.Cross(currentVelocity, Vector3.up); // this is the vertical plane that intersects current velocity

        // Convert above desired direction into actual direction along survace beneath
        RaycastHit groundHitInfo;
        RaycastHit headHitInfo;
        bool isOnGround = //characterController.isGrounded;
        Physics.SphereCast(transform.position + new Vector3(0.0f, height / 2, 0.0f), radius - 0.1f, Vector3.down, out groundHitInfo, // -0.1f so that this collision doesn't detect stuff that the collider detects on sides
                           (height / 2) - radius + 0.2f, Physics.AllLayers, QueryTriggerInteraction.Ignore); // 0.2f is a buffer because character controller may register collision before this cast does


        bool hitHead = Physics.SphereCast(transform.position, radius -0.1f, Vector3.up, out headHitInfo, //-0.1f so that this collision doesn't detect stuff that the collider detects on sides
                           height /*/ 2 */ - radius, Physics.AllLayers, QueryTriggerInteraction.Ignore);


        if (currentJumpState == JumpState.Inititated && previousJumpState == JumpState.Grounded) {
            Vector2 xzPlaneVelocity = Vector2.ClampMagnitude(new Vector2(currentVelocity.x, currentVelocity.z), GetMaxXZAirborneVelocity()); // clamp speed along xz plane
            currentVelocity = new Vector3(xzPlaneVelocity.x, jumpCoefficient, xzPlaneVelocity.y); // jumping

            // TODO: play jump noise
        }
        else if (currentJumpState == JumpState.Inititated && hitHead) {
            currentJumpState = JumpState.Grounded; // could also choose just landed and play noises for both leaving ground and landing; or neither
        }
        else if (currentJumpState == JumpState.Inititated && !isOnGround) { // just left the ground
            currentJumpState = JumpState.Airborn;
        }
        else if (currentJumpState == JumpState.Airborn) {
            if (isOnGround) {
                currentJumpState = JumpState.JustLanded;
                timeLanded = Time.time; // timestamp of when landed
                // TODO: play land noise
            }
            else {
                desiredAcceleration += Physics.gravity;
                desiredAcceleration += (transform.forward * zMovement + transform.right * xMovement) * airbornAccelRate;
                //Vector2 xzPlaneVelocity = Vector2.ClampMagnitude(new Vector2(velocity.x, velocity.z), jumpSpeed);
                Vector2 xzPlaneVelocity = Vector2.ClampMagnitude(new Vector2(currentVelocity.x, currentVelocity.z), GetMaxXZAirborneVelocity()); // clamp speed along xz plane
                Debug.Log("Acceleration: " + desiredAcceleration);
                currentVelocity = new Vector3(xzPlaneVelocity.x, currentVelocity.y, xzPlaneVelocity.y) + (desiredAcceleration * Time.deltaTime);
            }
            
        }
        else if (!isOnGround) {
            // TODO: figure out why this happens
            // Debug.Log("I am not on the ground, somehow.");
            currentJumpState = JumpState.Airborn;
        }
        else if (currentJumpState == JumpState.JustLanded) {
            if (Time.time - timeLanded > jumpLandDuration) {
                currentJumpState = JumpState.Grounded;
                timeLanded = 0.0f; // sentinel value indicating not in use
            }
        }
        if (currentJumpState == JumpState.Grounded || currentJumpState == JumpState.JustLanded) {
            Vector3 groundHitNormal = groundHitInfo.normal.normalized;
            float angleFromVertical = MathUtil.convertRadToDegree(Mathf.Acos(Vector3.Dot(groundHitNormal, Vector3.up)));
            velocityDirectionPlane = Vector3.Cross(currentVelocity, Vector3.up);
            Vector3 velocityDirection = Vector3.Cross(groundHitInfo.normal, velocityDirectionPlane).normalized; // line of intersection between vertical plane of velocity and plane walking on.
            if (angleFromVertical > characterController.slopeLimit) { // slide down the terrain bc its too steep
                velocityDirectionPlane = Vector3.Cross(groundHitNormal, Vector3.up);
                velocityDirection = Vector3.Cross(groundHitNormal, velocityDirectionPlane).normalized;
                currentVelocity = velocityDirection * slideDownSlopeSpeed;
            }
            else {
                accelDirectionPlane = Vector3.Cross(desiredAccelerationDirection, Vector3.up); // this is the plane that intersets both y axis and movement direction, always vertical
                desiredAccelerationDirection = Vector3.Cross(groundHitInfo.normal, accelDirectionPlane).normalized; // this is the line of intersection between the vertical plane and the plane walking on.
                desiredAccelerationDirection = desiredAccelerationDirection.normalized; // normalize the direction of the accel
                desiredAcceleration = desiredAccelerationDirection * accelRate; // now get actual acceleration by taking direction * accelRate
            }

            Vector3 friction = velocityDirection * frictionCoefficient * -1;
            if ((friction.magnitude * Time.fixedDeltaTime) > currentVelocity.magnitude && desiredAcceleration.magnitude == 0) { // friction would actually cause velocity in negative direction
                friction = -currentVelocity / Time.fixedDeltaTime; // so make it so that velocity will be zero due to friction
            }
            currentVelocity = currentVelocity.magnitude * velocityDirection + (desiredAcceleration + friction) * Time.deltaTime;
            /*if (velocity.magnitude > sprintSpeed) { // can't go faster than max velocity
                velocity = Vector3.ClampMagnitude(velocity, sprintSpeed);
            }*/
        }
        else {
        }

        Vector3 moveVal = currentVelocity * Time.deltaTime;

        CmdMove(moveVal); // call to move on the server
        UpdatePreviousStates();
    }

    [Command]
    public void CmdMove(Vector3 displacement) {
        collisionFlags = characterController.Move(displacement);
    }

    public void SetRunState (RunState runStateIn) {
        currentRunState = runStateIn;
    }

    public void SetMovementDirection(Vector2 movement) {
        zMovement = movement.y;
        xMovement = movement.x;
    }

    public void Jump() {
        if (previousJumpState == JumpState.Grounded) {
            currentJumpState = JumpState.Inititated;
        }
    }
    public bool ToggleCrouch() {
        if (currentCrouchState == CrouchState.Standing) {
            currentCrouchState = CrouchState.Crouched;
        } else {
            currentCrouchState = CrouchState.Standing;
        }
        return IsCrouching();
    }

    public bool IsCrouching() {
        bool isCrouching = false;
        if (previousCrouchState == CrouchState.Crouched) {
            isCrouching = true;
        }
        return isCrouching;
    }
    public bool IsJumping() {
        bool isJumping = false;
        if (previousJumpState == JumpState.Airborn || previousJumpState == JumpState.Inititated || previousJumpState == JumpState.JustLanded) {
            isJumping = true;
        }
        return isJumping;
    }
    public bool IsSprinting() {
        return previousRunState == RunState.Sprinting;
    }
    public bool IsRunning() {
        return previousRunState == RunState.Running;
    }
    public bool IsWalking() {
        return previousRunState == RunState.Walking;
    }
    public bool IsStill() {
        return previousRunState == RunState.Still;
    }
    public bool IsBackPedaling() {
        return false; // TODO: implement
    }
    public bool IsStrafing() {
        return previouslyWasStrafing;
    }


    private float GetJumpHeight () {
        return jumpHeight;
    }

    /// <summary>
    /// Gets the maximum velocity that this player can move in xz plane while in air
    /// </summary>
    /// <returns></returns>
    private float GetMaxXZAirborneVelocity () {
        return maxXZAirborneSpeed;
    }

    private void UpdatePreviousStates () {
        previousJumpState = currentJumpState;
        previouslyWasStrafing = currentlyIsStrafing;
        previousCrouchState = currentCrouchState;
        previousRunState = currentRunState;
    }

    public Vector3 GetCurrentVelocity () {
        return characterController.velocity;
    }

    public enum JumpState {
        Inititated
    , Airborn
    , JustLanded
    , Grounded
    };

    public enum CrouchState {
        Standing
        , Crouched
    }

    public enum RunState {
        Sprinting
        , Running
        , Walking
        , Still
    }

    public enum RelativeDirection {
        Left
        , Right
        , Forward
        , Backward
        , Up
        , Down
    }
}
