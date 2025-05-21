using UnityEngine;

public interface ICommand
{
    public void Execute();
}

public class MoveCommand : ICommand
{
    PlayerController player;
    public MoveCommand(PlayerController player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.stateMachine.ChangeState(new MoveState(player));
    }
}

public class IdleCommand : ICommand
{
    PlayerController player;
    public IdleCommand(PlayerController player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.stateMachine.ChangeState(new IdleState());
    }
}

public class AttackCommand : ICommand
{
    PlayerController player;

    // PlayerAttackNonTargeting attack;

    PlayerAttackIK attack;

    public AttackCommand(PlayerController player, PlayerAttackIK attack)
    {
        this.player = player;
        this.attack = attack;
    }

    public void Execute()
    {
        attack.Perform();
    }
}

public class SkillCastCommand : ICommand
{
    PlayerController caster;
    public GameObject mark { get; private set; }
    
    public SkillCastCommand(PlayerController caster)
    {
        SkillData skillData = caster.data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        mark = GameObject.Instantiate(skillData.castingMark, caster.transform);
        mark.SetActive(false);
    }

    public void Execute()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mark.activeSelf) SetMark(false);
            else SetMark(true);
        }
        
        if (!mark.activeSelf) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            mark.transform.position = hit.point;
        }
    }
    
    public void SetMark(bool on)
    {
        mark.SetActive(on);
    }
}

public class SkillCommand : ICommand
{
    PlayerController player;

    ISkillAction skillAction;

    public SkillCommand(PlayerController player, ISkillAction skillAction)
    {
        this.player = player;
        this.skillAction = skillAction;
    }

    public void Execute()
    {
        skillAction.Perform();
    }
}


public class DetectionCommand : ICommand
{
    PlayerController player;
    PlayerDetection detection;

    bool detectionState;

    public DetectionCommand(PlayerController player, PlayerDetection detection)
    {
        this.player = player;
        this.detection = detection;
        detectionState = false;
    }

    public void OnOff(bool on)
    {
        detectionState = on;
        player.lineRenderer.enabled = on;
    }

    public void Execute()
    {
        if (!detectionState) return;

        detection.Perform();
    }
}