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
    private GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Fire(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
        StartSkillSequence(point);
    }

    void StartSkillSequence(Vector3 point)
    {
        if (skillEffectObject == null) return;

        skillEffectObject.transform.position = point;

        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(data.castingTime)
                .AppendCallback(() => skillEffectObject.SetActive(true))
                .AppendInterval(data.duration)
                .AppendCallback(() => skillEffectObject.SetActive(false));
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
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Frost(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectController>().SetProps(caster, data);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
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
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Heal(PlayerController_Net caster, SkillData data)
    {

        this.caster = caster;
        this.data = data;

        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectController>().SetProps(caster, data);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
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

        if (data.prefab == null)
        {
            Debug.LogError("[Skill_YellowRod] SkillData의 prefab이 설정되어 있지 않습니다!");
            return;
        }
    
        skillEffectObject = GameObject.Instantiate(data.prefab);
        skillEffectObject.GetComponent<SkillEffectController>().SetProps(caster, data);
        skillEffectObject.SetActive(false);
    }

    public void Perform(Vector3 point)
    {
        StopCaster();
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
    void StopCaster()
    {
        Sequence stopSeq = DOTween.Sequence();

        stopSeq.AppendCallback(() => caster.inputHandler.isStop = true)
                .AppendInterval(2f)
                .AppendCallback(() => caster.inputHandler.isStop = false);
    }
}