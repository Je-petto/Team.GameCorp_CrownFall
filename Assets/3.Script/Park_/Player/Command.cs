using DG.Tweening;
using UnityEngine;

public interface ICommand
{
    public void Execute();
}

public class DeathCommand : ICommand
{
    PlayerController_Net player;
    DeadState state;
    public DeathCommand(PlayerController_Net player, DeadState state)
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
    PlayerController_Net player;
    public MoveCommand(PlayerController_Net player)
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
    PlayerController_Net player;
    public IdleCommand(PlayerController_Net player)
    {
        this.player = player;
    }

    public void Execute()
    {
        if (player.teamCode != 0) return;
        player.stateMachine.ChangeState(new IdleState(player));
    }
}

public class DetectCommand : ICommand
{
    PlayerController_Net player;
    PlayerDetection detection;

    bool detectionState;

    public DetectCommand(PlayerController_Net player, PlayerDetection detection)
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
    PlayerController_Net player;

    PlayerAttackNonTargeting attack;

    public AttackCommand(PlayerController_Net player, PlayerAttackNonTargeting attack)
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
    PlayerController_Net caster;
    public GameObject mark { get; private set; }
    ISkillAction skillAction;
    SkillData data;
    public bool isCasting { get; private set; }

    private bool isCoolDown = false;

    public SkillCastCommand(PlayerController_Net caster, SkillData data, ISkillAction skillAction)
    {
        Debug.Log("[Client] SkillCast Set Complete");

        this.data = data;
        this.caster = caster;
        isCasting = false;

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


        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            mark.transform.position = hit.point;
            caster.targetPoint = hit.point;
        }

        //스킬 실행!
        if (Input.GetMouseButtonDown(0))
        {
            if (skillAction == null) return;

            if (caster.animator != null) caster.animator.SetTrigger("Skill");

            Vector3 lookPoint = new Vector3(caster.targetPoint.x, 0f, caster.targetPoint.z);
            caster.transform.LookAt(lookPoint);
            skillAction.Perform(caster.targetPoint);
            mark.SetActive(false);

            SkillCoolDown(data.coolDown);

            SetCasting();
        }
    }
    private void SkillCoolDown(float cooldown)
    {
        Sequence coolSeq = DOTween.Sequence();

        caster.skillCoolDownImage.fillAmount = 1f;
        
        coolSeq.AppendCallback(() => isCoolDown = true).Append(caster.skillCoolDownImage.DOFillAmount(0f, cooldown).SetEase(Ease.Linear))
                    .OnComplete(() => isCoolDown = false);
    }

    private void SetCasting()
    {
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(0.1f).OnComplete(() => isCasting = false);
    }
}