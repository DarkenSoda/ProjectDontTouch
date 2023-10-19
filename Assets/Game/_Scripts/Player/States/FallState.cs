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
        }

        // WallRun and WallStand
    }

    public override void EnterState() {
        InitializeSubState();

        context.Anim.SetBool("IsFalling", true);
        context.Anim.CrossFade("Falling Idle", .15f, 0, 0);
    }

    public override void UpdateState() {
        CheckSwitchState();
    }

    public override void ExitState() {
        context.Anim.SetBool("IsFalling", false);
    }
}
