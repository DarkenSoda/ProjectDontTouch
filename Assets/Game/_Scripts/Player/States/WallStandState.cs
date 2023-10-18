using UnityEngine;

public class WallStandState : BaseState {
    public WallStandState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
        isRootState = true;
    }

    public override void CheckSwitchState() {
    
    }

    public override void UpdateState() {
        CheckSwitchState();
    }
}
