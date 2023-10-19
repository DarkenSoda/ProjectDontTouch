using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashState : BaseState {
    private float startTime;

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
        } else {
            SwitchState(stateFactory.Fall());
        }
    }

    public override void EnterState() {
        context.DesiredSpeed = context.DashSpeed;
        startTime = Time.time;
        context.RB.drag = 0;
        context.RB.useGravity = false;

        Dash();

        context.Anim.CrossFade("Dash", .15f, 0, 0);
    }

    public override void UpdateState() {
        context.visuals.DOLookAt(context.visuals.position + context.orientation.forward, .2f, AxisConstraint.Y);

        if (Time.time - startTime > context.DashDuration) {
            CheckSwitchState();
        }
    }

    public override void ExitState() {
        context.RB.useGravity = true;
    }

    private void Dash() {
        context.RB.velocity = new Vector3(context.RB.velocity.x, 0, context.RB.velocity.z);

        Vector3 direction = context.orientation.forward * context.DashPower + context.orientation.up * context.VerticalDashPower;
        context.RB.AddForce(direction, ForceMode.Impulse);

        context.DashCooldown = context.DashCooldownMax;
    }
}
