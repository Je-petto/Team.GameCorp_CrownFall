using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillControl : MonoBehaviour
{
    [Header("Skill CoolDown")]
    [SerializeField] private bool isCoolDown = false;

    [Header("Using Skill Type")]
    [SerializeField] private SkillType skillType = SkillType.NONE;

    [Header("SkillData")]
    [SerializeField] private SkillData skillData;
    [SerializeField] private GameObject skillRef;
    private GameObject skill;

    [Header("Ray")]
    public LineRenderer line;


    [Header("Target")]
    [SerializeField] private float targetHp;
    [SerializeField] private float targetMoveSpeed;
    [SerializeField] private float targetAttackSpeed;


    private void Awake()
    {
        Initialize();
    }

    private void Update()
    {
        SkillClick();
        SkillRay();
    }

    private void Initialize()
    {
        skillRef = skillData.model;
        skillType = skillData.type;
        skill = Instantiate(skillRef);
        skill.SetActive(false);
    }

    private void SkillClick()
    {
        if (skillType.Equals(SkillType.YELLOW)) return;

        if (Input.GetMouseButtonDown(1) && !isCoolDown)
        {
            Debug.Log("Click");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isCoolDown = true;
                skill.transform.position = hit.point;
                skill.SetActive(true);
                StartCoroutine(SkillDuration_Co());
                StartCoroutine(SkillCoolDown_Co());
            }
        }
    }

    private void SkillRay()
    {
        if (!skillType.Equals(SkillType.YELLOW)) return;

        Vector3 hitPos;
        if (Input.GetMouseButtonDown(1) && !isCoolDown)
        {
            Debug.Log("Ray");

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
            {
                isCoolDown = true;
                //skill.transform.position = hit.point;
                //skill.SetActive(true);
                //StartCoroutine(SkillDuration_Co());
                //StartCoroutine(SkillCoolDown_Co());
                hitPos = hit.point;
            }
            else
                hitPos = transform.position + transform.forward * 50f;

            StartCoroutine(ShotEffect(hitPos));
        }
    }

    // 스킬 지속시간
    IEnumerator SkillDuration_Co()
    {
        yield return new WaitForSeconds(skillData.duration);
        skill.SetActive(false);
    }

    // 스킬 시전 후 다음 스킬 시전까지 쿨타임
    IEnumerator SkillCoolDown_Co()
    {
        yield return new WaitForSeconds(skillData.coolDown);
        isCoolDown = false;
    }

    // 지속 데미지
    IEnumerator ContinuousDamage_Co()
    {
        for (int i = 0; i < skillData.effectDuration;)
        {
            targetHp -= skillData.damage;
            yield return new WaitForSeconds(skillData.effectDuration / skillData.effectDuration);
            i++;
        }
    }

    IEnumerator SlowSpeed_Co()
    {
        float tempMS = targetMoveSpeed;
        float tempAS = targetAttackSpeed;

        targetMoveSpeed *= 0.75f;
        targetAttackSpeed *= 0.75f;
        yield return new WaitForSeconds(skillData.effectDuration);
        targetMoveSpeed = tempMS;
        targetAttackSpeed = tempAS;
    }

    private IEnumerator ShotEffect(Vector3 point)
    {

        line.SetPosition(0, transform.position);
        line.SetPosition(1, point);

        line.enabled = true;
        yield return new WaitForSeconds(0.03f);
        line.enabled = false;
    }
}