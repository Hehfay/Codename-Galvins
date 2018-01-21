using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ActorTranslationInterface {


    void SetRunState(PlayerTranslation.RunState runStateIn);
    void SetMovementDirection(Vector2 movement);
    void Jump();
    bool ToggleCrouch();

    bool IsSprinting();
    bool IsRunning();
    bool IsWalking();
    bool IsStill();
    bool IsBackPedaling();
    bool IsStrafing();
    bool IsJumping();
    bool IsCrouching();
}
