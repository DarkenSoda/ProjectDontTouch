using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashState : BaseState {
    private float startTime;
    private Vector3 direction;

    public DashState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
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
        } else if (context.IsNearWall && context.IsFarFromGround) {
            if (context.AngleBetweenWall <= context.WallStandAngle) {
                SwitchState(stateFactory.WallStand());
            } else {
                // SwitchState(stateFactory.WallRun());
            }
        } else {
            SwitchState(stateFactory.Fall());
        }
    }

    public override void EnterState() {
        context.DesiredSpeed = context.DashSpeed;
        startTime = Time.time;
        context.RB.drag = 0;
        context.ApplyGravity = false;
        context.ApplyRotation = false;

        Dash();

        context.Anim.CrossFade("Dash", .15f, 0, 0);
    }

    public override void UpdateState() {
        context.visuals.DOLookAt(context.visuals.position + direction, .2f, AxisConstraint.Y);

        if (Time.time - startTime > context.DashDuration) {
            CheckSwitchState();
        }
    }

    public override void ExitState() {
        context.ApplyGravity = true;
        context.ApplyRotation = true;

        context.DashCooldown = context.DashCooldownMax;
    }

    private void Dash() {
        direction = context.MoveDir;
        if (direction == Vector3.zero) {
            direction = context.orientation.forward * context.DashPower + context.orientation.up * context.VerticalDashPower;
        } else {
            direction.y = 0;
            direction = direction.normalized * context.DashPower;
            direction.y = context.VerticalDashPower;
        }

        context.RB.velocity = Vector3.zero;
        context.RB.AddForce(direction, ForceMode.Impulse);
    }
}
