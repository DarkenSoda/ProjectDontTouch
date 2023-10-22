using System.Net.Mime;
using UnityEngine;

public class JumpState : BaseState {
    private float startTime;    // To fix issue where jump transition to ground immediately
    public JumpState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
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
        if (!context.IsGrounded && context.RB.velocity.y < 0) {
            SwitchState(stateFactory.Fall());
        } else if (context.IsDashingPressed && context.DashCooldown <= 0) {
            SwitchState(stateFactory.Dash());
        } else if (context.IsGrounded && Time.time - startTime > .5f) {
            SwitchState(stateFactory.Ground());
        }

        // WallRun and WallStand
        else if (context.IsNearWall && context.IsFarFromGround) {
            if (context.AngleBetweenWall <= context.WallStandAngle) {
                SwitchState(stateFactory.WallStand());
            } else {
                SwitchState(stateFactory.WallRun());
            }
        }
    }

    public override void EnterState() {
        InitializeSubState();

        Jump();
        context.RequireNewJumpPress = true;

        context.RB.drag = 0;
        startTime = 0;

        context.Anim.CrossFade("Jumping Up", .15f, 0, 0);

    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    private void Jump() {
        context.RB.velocity = new Vector3(context.RB.velocity.x, 0, context.RB.velocity.z);
        context.RB.AddForce(context.transform.up * context.JumpForce, ForceMode.Impulse);
    }
}
