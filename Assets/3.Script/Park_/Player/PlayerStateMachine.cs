using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState currentState;

    void Start()
    {
        
    }

    public void ChangeState(IPlayerState state)
    {
        // if (!isLocalPlayer) return;

        if (state.Equals(currentState)) return;

        currentState?.Exit();
        currentState = state;
        currentState.Enter();
    }

    public void FixedUpdate()
    {
        // if (!isLocalPlayer) return;

        currentState?.Update();
    }
}