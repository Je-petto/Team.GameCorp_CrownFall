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
    [SerializeField] private SkillData data;
    [SerializeField] private GameObject prefab;

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

    List<IEffect> effects = new List<IEffect>();

    private void Initialize()
    {
        line.enabled = false;
        skillType = data.type;
        prefab = Instantiate(data.prefab);
        prefab.SetActive(false);


        //effects = EffectFactory.CreateEffects(data.effects);
    }

    private void SkillClick()
    {
        if (skillType.Equals(SkillType.YELLOW)) return;

        if (Input.GetMouseButtonDown(1) && !isCoolDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                isCoolDown = true;
                prefab.transform.position = hit.point;
                prefab.SetActive(true);
                StartCoroutine(SkillDuration_Co());
                StartCoroutine(SkillCoolDown_Co());
            }
        }
    }

    private void SkillRay()
    {
        if (!skillType.Equals(SkillType.YELLOW)) return;

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

            StartCoroutine(ShotEffect(hitPos));

        }
    }

    // 스킬 지속시간
    IEnumerator SkillDuration_Co()
    {
        yield return new WaitForSeconds(data.duration);
        prefab.SetActive(false);
    }

    // 스킬 시전 후 다음 스킬 시전까지 쿨타임
    IEnumerator SkillCoolDown_Co()
    {
        yield return new WaitForSeconds(data.coolDown);
        isCoolDown = false;
    }

    // 지속 데미지
    IEnumerator DotDamage_Co()
    {
        for (int i = 0; i < data.dot;)
        {
            targetHp -= data.damage;
            yield return new WaitForSeconds(data.dot / data.dot);
            i++;
        }
    }

    IEnumerator SlowSpeed_Co()
    {
        float tempMS = targetMoveSpeed;
        float tempAS = targetAttackSpeed;

        targetMoveSpeed *= 0.75f;
        targetAttackSpeed *= 0.75f;
        yield return new WaitForSeconds(data.dot);
        targetMoveSpeed = tempMS;
        targetAttackSpeed = tempAS;
    }

    private IEnumerator ShotEffect(Vector3 point)
    {

        line.SetPosition(0, transform.position);
        line.SetPosition(1, point);

        line.enabled = true;
        yield return new WaitForSeconds(data.duration);
        line.enabled = false;
    }
}