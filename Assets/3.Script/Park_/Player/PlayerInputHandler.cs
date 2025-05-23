using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public ICommand moveCommand;
    public ICommand attackCommand;
    public ICommand detectCommand;                  // Debugging T
    public ICommand skillCastCommand;
    public ICommand deathCommand;
    private SkillCastCommand castCommand => skillCastCommand as SkillCastCommand;

    bool isDeath = false;           //test

    void Update()
    {
        // if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            deathCommand.Execute();
        }

        skillCastCommand.Execute();

        if (!castCommand.isCasting)
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
        }
    }

    void FixedUpdate()
    {
        // if (!isLocalPlayer) return;
        if (moveCommand == null) return;
        moveCommand.Execute();
    }
}