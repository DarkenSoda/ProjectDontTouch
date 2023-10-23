using UnityEngine;

public class WallRunState : BaseState {
    private bool isWallJumping;
    private Vector3 wallNormal;
    private Vector3 wallForward;
    private Vector3 rotationDirection;
    private bool canDetectWall;

    public WallRunState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
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
        } else if (!canDetectWall) {
            SwitchState(stateFactory.Fall());
        }
    }

    public override void EnterState() {
        isWallJumping = false;
        context.ApplyGravity = false;
        context.ApplyRotation = false;

        wallNormal = context.WallHit.normal;
        wallForward = Vector3.Cross(wallNormal, context.transform.up);
        if ((context.visuals.forward - wallForward).magnitude > (context.visuals.forward - -wallForward).magnitude) {
            wallForward *= -1;
        }

        context.Anim.CrossFade("Leaning", .15f, 0, 0);  // temp animation
        context.visuals.rotation = Quaternion.LookRotation(wallNormal); // temp rotation
        context.RB.AddForce(-wallNormal * 100, ForceMode.Impulse);
    }

    public override void UpdateState() {
        canDetectWall = Physics.Raycast(context.transform.position, -wallNormal, context.WallDetectionDistance, context.WallLayer);

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
            context.RB.AddForce(wallForward.normalized * context.WallRunSpeed, ForceMode.Force);

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

        Vector3 direction = Vector3.up + wallNormal + wallForward;
        rotationDirection = new Vector3(direction.x, 0, direction.z);
        context.RB.AddForce(direction * context.WallJumpForce, ForceMode.Impulse);

        context.Anim.CrossFade("WallJump", .15f, 0, 0);
    }
}
