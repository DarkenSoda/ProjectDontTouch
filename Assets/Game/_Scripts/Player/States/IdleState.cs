using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState {
    public IdleState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) { }

    public override void CheckSwitchState() {
        if(context.IsMoving) {
            SwitchState(stateFactory.Move());
        }
    }

    public override void UpdateState() {
        context.Anim.SetFloat("Speed", 0, .1f, Time.deltaTime);

        CheckSwitchState();
    }
}
