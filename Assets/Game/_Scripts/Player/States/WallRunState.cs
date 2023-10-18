using UnityEngine;

public class WallRunState : BaseState {
    public WallRunState(PlayerContext context, StateFactory stateFactory) : base(context, stateFactory) {
        isRootState = true;
    }

    public override void CheckSwitchState() {
    
    }

    public override void UpdateState() {
        CheckSwitchState();
    }
}
