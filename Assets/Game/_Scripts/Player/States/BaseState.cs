public abstract class BaseState {
    protected PlayerContext context;
    protected StateFactory stateFactory;
    protected BaseState currentSuperState;
    protected BaseState currentSubState;
    protected bool isRootState;

    public BaseState(PlayerContext context, StateFactory stateFactory) {
        this.context = context;
        this.stateFactory = stateFactory;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void FixedUpdateState() { }
    public abstract void UpdateState();
    public virtual void InitializeSubState() { }
    public abstract void CheckSwitchState();

    public void UpdateStates() {
        UpdateState();

        if (currentSubState != null) {
            currentSubState.UpdateState();
        }
    }

    public void FixedUpdateStates() {
        FixedUpdateState();

        if (currentSubState != null) {
            currentSubState.FixedUpdateState();
        }
    }

    protected void SwitchState(BaseState newState) {
        ExitState();

        newState.EnterState();

        if (isRootState) {
            context.CurrentState = newState;
        } else if (currentSuperState != null) {
            currentSuperState.SetSubState(newState);
        }
    }

    protected void SetSuperState(BaseState newSuperState) {
        currentSuperState = newSuperState;
    }

    protected void SetSubState(BaseState newSubState) {
        currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}
