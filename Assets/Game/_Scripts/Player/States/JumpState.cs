using UnityEngine;

public class JumpState : BaseState {
    private bool isJumping;
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
        if (context.IsGrounded && !isJumping) {
            SwitchState(stateFactory.Ground());
        } else if (!context.IsGrounded && context.RB.velocity.y < 0) {
            SwitchState(stateFactory.Fall());
        } else if (context.IsDashingPressed && context.DashCooldown <= 0) {
            SwitchState(stateFactory.Dash());
        }

        // WallRun and WallStand
    }

    public override void EnterState() {
        InitializeSubState();

        context.RequireNewJumpPress = true;

        context.RB.drag = 0;
        isJumping = true;

        context.Anim.CrossFade("Jumping Up", .15f, 0, 0);

        Jump();
    }

    public override void UpdateState() {
        if(context.RB.velocity.y < 0) {
            isJumping = false;
        }

        CheckSwitchState();
    }

    private void Jump() {
        context.RB.velocity = new Vector3(context.RB.velocity.x, 0, context.RB.velocity.z);
        context.RB.AddForce(context.transform.up * context.JumpForce, ForceMode.Impulse);
    }
}
