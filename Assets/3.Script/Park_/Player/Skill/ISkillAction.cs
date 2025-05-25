using DG.Tweening;
using UnityEngine;


public interface ISkillAction
{
    public void Perform(Vector3 point);
}

//장판 소환
public class Skill_Fire : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    private GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Fire(PlayerController caster, SkillData data)
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
                .OnComplete(() => Debug.Log("Fire Cast Complete"));
    }
}

//장판 소환
public class Skill_Frost : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Frost(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectConroller>().SetProps(caster, data);
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
public class Skill_Heal : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Heal(PlayerController caster, SkillData data)
    {

        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectConroller>().SetProps(caster, data);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
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

public class Skill_YellowRod : ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;
    private bool isCoolDown;

    public Skill_YellowRod(PlayerController caster, SkillData data)
    {
                this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectConroller>().SetProps(caster, data);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        Debug.Log("Yellow Skill cast");
        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(data.castingTime)
                .AppendCallback(() => skillEffectObject.SetActive(true))
                .AppendCallback(() => skillEffectObject.transform.position = point)
                .AppendInterval(data.duration)
                .AppendCallback(() => skillEffectObject.SetActive(false));
    }
}