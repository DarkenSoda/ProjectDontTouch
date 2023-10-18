using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : BaseState {
    public MoveState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) { }

    public override void CheckSwitchState() {
        if(!context.IsMoving) {
            SwitchState(stateFactory.Idle());
        }
    }

    public override void UpdateState() {
        context.Anim.SetFloat("Speed", 1, .1f, Time.deltaTime);

        CheckSwitchState();
    }

    public override void FixedUpdateState() {
        HandleMovement();
    }

    private void HandleMovement() {
        if (context.IsGrounded) {
            context.RB.AddForce(context.MoveDir.normalized * context.MoveSpeed * context.SpeedMultiplier, ForceMode.Force);
        } else {
            context.RB.AddForce(context.MoveDir.normalized * context.MoveSpeed * context.AirMultiplier * context.SpeedMultiplier, ForceMode.Force);
        }
    }
}
