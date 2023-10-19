using UnityEngine;

public class JumpState : BaseState {
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
        if (context.IsGrounded) {
            SwitchState(stateFactory.Ground());
        } else if (!context.IsGrounded && context.RB.velocity.y < 0) {
            SwitchState(stateFactory.Fall());
        }

        // WallRun and WallStand
    }

    public override void EnterState() {
        InitializeSubState();

        context.RequireNewJumpPress = true;

        context.Anim.CrossFade("Jumping Up", .15f, 0, 0);

        Jump();
    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    private void Jump() {
        context.RB.velocity = new Vector3(context.RB.velocity.x, 0, context.RB.velocity.z);
        context.RB.AddForce(context.transform.up * context.JumpForce, ForceMode.Impulse);
    }
}
