using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    private IPlayerState currentState;

    void Start()
    {
        ChangeState(new IdleState());
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
        // 테스트
        Debug.Log($"{gameObject.GetComponent<Rigidbody>().velocity}");
        currentState?.Update();
    }
}