using UnityEngine;

public class FallState : BaseState {
    public FallState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
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
        if(context.IsGrounded) {
            SwitchState(stateFactory.Ground());
        } else if(context.IsDashingPressed && context.DashCooldown <= 0) {
            SwitchState(stateFactory.Dash());
        }

        // WallRun and WallStand
        else if (context.IsNearWall && context.IsFarFromGround) {
            if (context.AngleBetweenWall <= context.WallStandAngle) {
                SwitchState(stateFactory.WallStand());
            } else {
                // SwitchState(stateFactory.WallRun());
            }
        }
    }

    public override void EnterState() {
        InitializeSubState();

        context.RB.drag = 0;

        context.Anim.SetBool("IsFalling", true);
    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    public override void ExitState() {
        context.Anim.SetBool("IsFalling", false);
    }
}
