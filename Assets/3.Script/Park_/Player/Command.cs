using DG.Tweening;
using UnityEngine;

public interface ICommand
{
    public void Execute();
}

public class DeathCommand : ICommand
{
    PlayerController player;
    DeadState state;
    public DeathCommand(PlayerController player, DeadState state)
    {
        this.player = player;
        this.state = state;
    }

    public void Execute()
    {
        player.stateMachine.ChangeState(state);
    }
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
        player.stateMachine.ChangeState(new IdleState(player));
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

public class AttackCommand : ICommand
{
    PlayerController player;

    PlayerAttackNonTargeting attack;

    public AttackCommand(PlayerController player, PlayerAttackNonTargeting attack)
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
    ISkillAction skillAction;
    SkillData data;
    public bool isCasting { get; private set; }

    private bool isCoolDown = false;

    public SkillCastCommand(PlayerController caster, ISkillAction skillAction)
    {
        this.caster = caster;
        isCasting = false;

        data = caster.data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        mark = GameObject.Instantiate(data.castingMark, caster.transform);
        mark.SetActive(false);

        this.skillAction = skillAction;
    }

    public void Execute()
    {
        if (isCoolDown) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (mark.activeSelf)
            {
                mark.SetActive(false);
                isCasting = false;
            }
            else
            {
                mark.SetActive(true);
                isCasting = true;
            }
        }

        if (!mark.activeSelf) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Vector3 skillPoint = Vector3.zero;

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            mark.transform.position = hit.point;
            skillPoint = hit.point;
        }

        //스킬 실행!
        if (Input.GetMouseButtonDown(0))
        {
            if (skillAction == null) return;

            if (caster.animator != null) caster.animator.SetTrigger("Attack");

            skillAction.Perform(skillPoint);
            mark.SetActive(false);

            isCasting = false;
            
            SkillCoolDown(data.coolDown);
            SkillCast();
        }
    }


    private void SkillCast()
    {
        Sequence seq = DOTween.Sequence();

        //test...skilling
        seq.AppendCallback(() => caster.inputHandler.isStop = true)
            .AppendInterval(1f)
            .AppendCallback(() => caster.inputHandler.isStop = false);
        // 1초간 인풋 값 무시.
    }

    private void SkillCoolDown(float cooldown)
    {
        Sequence coolSeq = DOTween.Sequence();

        coolSeq.AppendCallback(() => isCoolDown = true).AppendInterval(cooldown)
                    .OnComplete(() => isCoolDown = false);
    }
}