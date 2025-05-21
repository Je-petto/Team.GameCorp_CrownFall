using System.Collections;
using UnityEngine;


public interface ISkillAction
{
    public void Perform();
}

//장판 소환
public class Skill_Red : MonoBehaviour, ISkillAction
{
    PlayerController caster;
    private SkillData data;
    public GameObject skillEffectObject;

    private bool isCoolDown;

    public Skill_Red(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        skillEffectObject = Instantiate(data.prefab);
        skillEffectObject.SetActive(false);
    }

    public void Perform()
    {
        StartCoroutine(SkillSequence_Co());
    }

    IEnumerator SkillSequence_Co()
    {
        Debug.Log("Red Cast...!");
        yield return new WaitForSeconds(data.castingTime);          //스킬 캐스팅 시간.
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

    public void Perform()
    {
        StartCoroutine(SkillSequence_Co());
    }

    IEnumerator SkillSequence_Co()
    {
        yield return new WaitForSeconds(data.castingTime);          //스킬 캐스팅 시간.

        skillEffectObject.SetActive(true);
        yield return new WaitForSeconds(data.duration);
        skillEffectObject.SetActive(true);
    }
}

public class Skill_Green : MonoBehaviour, ISkillAction
{
    PlayerController caster;
    private SkillData data;
    private bool isCoolDown;

    public Skill_Green(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform()
    {
        if (isCoolDown) return;
    }

    IEnumerator SkillSequence_Co()
    {
        yield return new WaitForSeconds(data.castingTime);
    }
}

public class Skill_Yellow : MonoBehaviour, ISkillAction
{
    PlayerController caster;
    private SkillData data;

    private bool isCoolDown;

    public Skill_Yellow(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    public void Perform()
    {
        if (isCoolDown) return;

        Vector3 hitPos;

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                isCoolDown = true;
                hitPos = hit.point - transform.position;
                hitPos.y = transform.position.y;
            }
            else
            {
                hitPos = transform.position + transform.position * data.distance;
                hitPos.y = transform.position.y;
            }

            StartCoroutine(SkillSequence_Co(hitPos));
        }

    }

    IEnumerator SkillSequence_Co(Vector3 point)
    {
        yield return new WaitForSeconds(data.castingTime);          //스킬 캐스팅 시간.

        caster.lineRenderer.SetPosition(0, transform.position);
        caster.lineRenderer.SetPosition(1, point);

        caster.lineRenderer.enabled = true;
        yield return new WaitForSeconds(data.duration);
        caster.lineRenderer.enabled = false;
    }
}