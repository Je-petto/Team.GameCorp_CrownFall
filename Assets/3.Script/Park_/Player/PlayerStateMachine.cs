using Mirror;

public class PlayerStateMachine : NetworkBehaviour
{
    private IPlayerState currentState;

    void Start()
    {
        ChangeState(new IdleState());
    }

    public void ChangeState(IPlayerState state)
    {
        if(!isLocalPlayer) return;

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void FixedUpdate()
    {
        if(!isLocalPlayer) return;
        
        currentState?.Update();
    }
}