using System.Collections;
using DG.Tweening;
using UnityEngine;


public interface ISkillAction
{
    public void Perform(Vector3 point);
}

//장판 소환
public class Skill_Red : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    private GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Red(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        Debug.Log("Red Skill_Case...");
        if (skillEffectObject == null) return;

        skillEffectObject.transform.position = point;

        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(data.castingTime)
                .AppendCallback(() => skillEffectObject.SetActive(true))
                .AppendInterval(1f)
                .AppendCallback(() => skillEffectObject.SetActive(false))
                .OnComplete(() => Debug.Log(",,,"));
    }
}

//장판 소환
public class Skill_Blue : MonoBehaviour, ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Blue(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = Instantiate(data.prefab);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        Debug.Log("Blue Skill_Case...");
        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(data.castingTime)
                .AppendCallback(() => skillEffectObject.SetActive(true))
                .AppendCallback(() => skillEffectObject.transform.position = point)
                .AppendInterval(data.duration)
                .AppendCallback(() => skillEffectObject.SetActive(false));
    }
}

// Heal
public class Skill_Green : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Green(PlayerController caster, SkillData data)
    {
        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.SetActive(false);

        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        if (isCoolDown) return;
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        Debug.Log("Green Skill cast");
        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(data.castingTime)
                .AppendCallback(() => skillEffectObject.SetActive(true))
                .AppendCallback(() => skillEffectObject.transform.position = point)
                .AppendInterval(data.duration)
                .AppendCallback(() => skillEffectObject.SetActive(false));
    }
}

public class Skill_Yellow : ISkillAction
{
    PlayerController caster;
    private SkillData data;

    private bool isCoolDown;

    public Skill_Yellow(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform(Vector3 point)
    {
        
    }
}