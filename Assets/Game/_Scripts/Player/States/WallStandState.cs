using DG.Tweening;
using UnityEngine;

public class WallStandState : BaseState {
    private bool isWallJumping;
    public WallStandState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
        isRootState = true;
    }

    public override void CheckSwitchState() {
        if (context.IsGrounded) {
            SwitchState(stateFactory.Ground());
        } else if(isWallJumping && context.RB.velocity.y < 0) {
            SwitchState(stateFactory.Fall());
        } else if(isWallJumping && context.IsDashingPressed && context.DashCooldown < 0) {
            SwitchState(stateFactory.Dash());
        }
    }

    public override void EnterState() {
        isWallJumping = false;
        context.ApplyGravity = false;
        context.ApplyRotation = false;

        // wall stand animation
        context.transform.DOLookAt(context.transform.position - context.WallHit.normal, .2f, AxisConstraint.Y);
    }

    public override void UpdateState() {
        if (!isWallJumping) {
            ApplyWallGravity();
        }

        if (!isWallJumping && context.IsJumpPressed && !context.RequireNewJumpPress) {
            WallJump();
        }
        
        if(isWallJumping) {
            context.transform.DOLookAt(context.transform.position + context.WallHit.normal, .2f, AxisConstraint.Y);
        }

        CheckSwitchState();
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
        Vector3 direction = Vector3.up + context.WallHit.normal;
        context.RB.AddForce(direction * context.WallJumpForce, ForceMode.Impulse);

        // walljump animation
    }
}
