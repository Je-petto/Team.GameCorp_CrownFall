using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public ICommand moveCommand;
    public ICommand attackCommand;
    public ICommand detectCommand;                  // Debugging T
    public ICommand skillCastCommand;
    public ICommand skillCommand;

    private bool isCasting = false;

    void Update()
    {
        // if (!isLocalPlayer) return;

        skillCastCommand.Execute();

        if (!isCasting)         //캐스팅 상태
        {
            if (Input.GetMouseButtonDown(0))
            {
                (detectCommand as DetectionCommand).OnOff(true);
            }

            if (Input.GetMouseButton(0))
            {
                detectCommand.Execute();
            }

            if (Input.GetMouseButtonUp(0))
            {
                (detectCommand as DetectionCommand).OnOff(false);
                attackCommand.Execute();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (skillCastCommand is SkillCastCommand command)
                {
                    if (command.mark.activeSelf) command.SetMark(false);
                    else command.SetMark(true);
                }
            }

        }
        else
        {
            if (Input.GetMouseButton(0)) skillCommand.Execute();
        }
    }

    void FixedUpdate()
    {
        // if (!isLocalPlayer) return;
        if (moveCommand == null) return;
        moveCommand.Execute();
    }
}
