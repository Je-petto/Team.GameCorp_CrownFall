using Unity.VisualScripting;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public ICommand moveCommand;
    public ICommand attackCommand;
    public ICommand detectCommand;
    void Update()
    {
        // if (!isLocalPlayer) return;

        if (Input.GetMouseButtonDown(0))
        {
            detectCommand.Execute();
            attackCommand.Execute();
        }
    }

    void FixedUpdate()
    {
        // if (!isLocalPlayer) return;
        if (moveCommand == null) return;
        moveCommand.Execute();
    }
}
