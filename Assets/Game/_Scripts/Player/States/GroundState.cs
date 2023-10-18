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
        }
    }

    public override void EnterState() {
        InitializeSubState();
    }

    public override void UpdateState() {
        context.DesiredSpeed = context.MoveSpeed;

        CheckSwitchState();
    }
}
