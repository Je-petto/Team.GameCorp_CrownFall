using DG.Tweening;
using UnityEngine;


public interface ISkillAction
{
    public void Perform(Vector3 point);
}

//장판 소환
public class Skill_Fire : ISkillAction
{
    PlayerController_Net caster;
    private SkillData data;

    public Skill_Fire(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        caster.CMDCastSkill(point);
    }
    
    void StopCaster()
    {
        Sequence stopSeq = DOTween.Sequence();

        stopSeq.AppendCallback(() => caster.inputHandler.isStop = true)
                .AppendInterval(2f)
                .AppendCallback(() => caster.inputHandler.isStop = false);
    }
}

//장판 소환
public class Skill_Frost : ISkillAction
{
    PlayerController_Net caster;
    private SkillData data;

    public Skill_Frost(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        caster.CMDCastSkill(point);
    }

    void StopCaster()
    {
        Sequence stopSeq = DOTween.Sequence();

        stopSeq.AppendCallback(() => caster.inputHandler.isStop = true)
                .AppendInterval(2f)
                .AppendCallback(() => caster.inputHandler.isStop = false);
    }
}

// Heal
public class Skill_Heal : ISkillAction
{
    PlayerController_Net caster;
    private SkillData data;

    public Skill_Heal(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        caster.CMDCastSkill(point);
    }
    void StopCaster()
    {
        Sequence stopSeq = DOTween.Sequence();

        stopSeq.AppendCallback(() => caster.inputHandler.isStop = true)
                .AppendInterval(2f)
                .AppendCallback(() => caster.inputHandler.isStop = false);
    }
}

public class Skill_YellowRod : ISkillAction
{
    PlayerController_Net caster;
    private SkillData data;
    public GameObject skillEffectObject;
    private bool isCoolDown;

    public Skill_YellowRod(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        caster.CMDCastSkill(point);
    }
    void StopCaster()
    {
        Sequence stopSeq = DOTween.Sequence();

        stopSeq.AppendCallback(() => caster.inputHandler.isStop = true)
                .AppendInterval(2f)
                .AppendCallback(() => caster.inputHandler.isStop = false);
    }
}