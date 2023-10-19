using System;
using Unity.VisualScripting;
using UnityEngine;

public class GroundState : BaseState {
    public GroundState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
        isRootState = true;
    }

    public override void InitializeSubState() {
        if (context.IsMoving) {
            SetSubState(stateFactory.Move());
        } else {
            SetSubState(stateFactory.Idle());
        }
    }

    public override void CheckSwitchState() {
        if (context.IsJumpPressed && !context.RequireNewJumpPress) {
            SwitchState(stateFactory.Jump());
        } else if (!context.IsGrounded) {
            SwitchState(stateFactory.Fall());
        } else if (context.IsDashingPressed && context.DashCooldown <= 0) {
            SwitchState(stateFactory.Dash());
        }
    }

    public override void EnterState() {
        InitializeSubState();

        context.RB.drag = context.GroundDrag;

        context.DesiredSpeed = context.MoveSpeed;

        context.Anim.SetBool("IsGrounded", true);
    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    public override void ExitState() {
        context.Anim.SetBool("IsGrounded", false);
    }
}
