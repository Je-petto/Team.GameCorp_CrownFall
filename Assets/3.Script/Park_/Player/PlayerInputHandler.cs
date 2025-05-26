using Mirror;
using UnityEngine;

public class PlayerInputHandler : NetworkBehaviour
{
    public ICommand moveCommand;
    public ICommand attackCommand;
    public ICommand detectCommand;                  // Debugging T
    public ICommand skillCastCommand;
    public ICommand deathCommand;
    private SkillCastCommand castCommand => skillCastCommand as SkillCastCommand;

    public bool isDeath;
    public bool isStop;

    void Start()
    {
        isDeath = false;
        isStop = false;
    }

    void Update()
    {
        if (!isLocalPlayer) return;  

        if (isStop || isDeath) return;

        if (castCommand == null)
        {
            Debug.Log($"[Client] : castCommand is null....");
            return;
        }

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

        if (skillCastCommand != null)
        {
            skillCastCommand.Execute();           
        }
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer) return;
        if (moveCommand == null) return;
        if (isStop) return;

        moveCommand.Execute();
    }
}