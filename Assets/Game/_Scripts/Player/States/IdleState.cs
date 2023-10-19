using UnityEngine;

public class IdleState : BaseState {
    public IdleState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) { }

    public override void CheckSwitchState() {
        if(context.IsMoving) {
            SwitchState(stateFactory.Move());
        }
    }

    public override void EnterState() {
        if (context.IsGrounded) {
            context.RB.velocity = new Vector3(0, context.RB.velocity.y, 0);
        }
    }

    public override void UpdateState() {
        context.Anim.SetFloat("Speed", 0, .1f, Time.deltaTime);

        CheckSwitchState();
    }
}
