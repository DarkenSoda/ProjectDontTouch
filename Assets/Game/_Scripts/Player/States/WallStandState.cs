using UnityEngine;

public class WallStandState : BaseState {
    private bool isWallJumping;
    private Vector3 wallNormal;
    private Vector3 rotationDirection;
    public WallStandState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
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
        } else if (isWallJumping && context.IsDashingPressed && context.DashCooldown < 0) {
            SwitchState(stateFactory.Dash());
        }
    }

    public override void EnterState() {
        isWallJumping = false;
        context.ApplyGravity = false;
        context.ApplyRotation = false;

        context.Anim.CrossFade("Wall Idle", .15f, 0, 0);
        wallNormal = context.WallHit.normal;
        context.RB.AddForce(-wallNormal * 100, ForceMode.Impulse);
        context.visuals.rotation = Quaternion.LookRotation(-wallNormal);
    }

    public override void UpdateState() {
        if (!isWallJumping && context.IsJumpPressed && !context.RequireNewJumpPress) {
            WallJump();

            InitializeSubState();
        }

        if (isWallJumping) {
            context.visuals.forward = Vector3.Slerp(context.visuals.forward, rotationDirection.normalized, context.RotationSpeed * Time.deltaTime);
        }

        CheckSwitchState();
    }

    public override void FixedUpdateState() {
        if (!isWallJumping) {
            ApplyWallGravity();
        }
    }

    public override void ExitState() {
        context.ApplyGravity = true;
        context.ApplyRotation = true;
    }

    private void ApplyWallGravity() {
        context.RB.velocity = Vector3.down * context.SlidingGravity;
    }

    private void WallJump() {
        isWallJumping = true;
        context.ApplyGravity = true;

        Vector3 direction = Vector3.up + wallNormal;
        rotationDirection = wallNormal;
        context.RB.AddForce(direction * context.WallJumpForce, ForceMode.Impulse);

        context.Anim.CrossFade("WallJump", .15f, 0, 0);
    }
}
