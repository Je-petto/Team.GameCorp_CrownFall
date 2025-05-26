using Mirror;
using UnityEngine;

public class PlayerStateMachine : NetworkBehaviour
{
    private IPlayerState currentState;

    void Start()
    {
        
    }

    public void ChangeState(IPlayerState state)
    {
        if (!isLocalPlayer) return;

        if (state.Equals(currentState)) return;

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        currentState?.Update();
    }
}