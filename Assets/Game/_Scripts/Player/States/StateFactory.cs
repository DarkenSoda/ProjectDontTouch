using System.Collections;
using System.Collections.Generic;

public class StateFactory {
    private enum OurStates {
        Idle, Move,
        Ground, Jump, Fall,
        WallRun,
        WallStand,
    }
    private PlayerContext context;
    private Dictionary<OurStates, BaseState> statesDictionary;
    public StateFactory(PlayerContext _context) {
        context = _context;
        statesDictionary = new Dictionary<OurStates, BaseState>();
        statesDictionary[OurStates.Idle] = new IdleState(context, this);
        statesDictionary[OurStates.Move] = new MoveState(context, this);
        statesDictionary[OurStates.Ground] = new GroundState(context, this);
        statesDictionary[OurStates.Jump] = new JumpState(context, this);
        statesDictionary[OurStates.Fall] = new FallState(context, this);
        statesDictionary[OurStates.WallRun] = new WallRunState(context, this);
        statesDictionary[OurStates.WallStand] = new WallStandState(context, this);
    }

    public BaseState Ground() {
        return statesDictionary[OurStates.Ground];
    }
    public BaseState Idle() {
        return statesDictionary[OurStates.Idle];
    }
    public BaseState Move() {
        return statesDictionary[OurStates.Move];
    }
    public BaseState Jump() {
        return statesDictionary[OurStates.Jump];
    }
    public BaseState Fall() {
        return statesDictionary[OurStates.Fall];
    }

    public BaseState WallRun() {
        return statesDictionary[OurStates.WallRun];
    }

    public BaseState WallStand() {
        return statesDictionary[OurStates.WallStand];
    }
}
