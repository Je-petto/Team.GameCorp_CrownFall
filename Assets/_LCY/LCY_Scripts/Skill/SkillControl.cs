using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillControl : MonoBehaviour
{
    [Header("Show")]
    [SerializeField] private GameObject show;

    [Header("Skill CoolDown")]
    [SerializeField] private bool isCoolDown = false;

    [Header("Using Skill Type")]
    [SerializeField] private SkillType skillType = SkillType.NONE;

    [Header("SkillData")]
    [SerializeField] private SkillData data;
    [SerializeField] private GameObject skillEffectPrefab;

    [Header("Ray")]
    public LineRenderer line;

    [Header("Target")]
    [SerializeField] private float targetHp;
    [SerializeField] private float targetMoveSpeed;
    [SerializeField] private float targetAttackSpeed;

    [SerializeField] private LayerMask layer;
    [SerializeField] private int healAmount;
    GameObject[] objectsWithTag;


    private void Awake()
    {
        Initialize();
    }

    void Start()
    {
        objectsWithTag = GameObject.FindGameObjectsWithTag($"{gameObject.tag}");
    }

    private void Update()
    {
        SkillClick();
        SkillRay();
        SkillHeal();
    }

    private void Initialize()
    {
        line.enabled = false;
        skillType = data.type;
        skillEffectPrefab = Instantiate(data.prefab);
        skillEffectPrefab.SetActive(false);
        show.SetActive(false);


        //effects = EffectFactory.CreateEffects(data.effects);
    }

    private void SkillClick()
    {
        if (skillType.Equals(SkillType.FROST) || skillType.Equals(SkillType.FIRE) || skillType.Equals(SkillType.LIGHTNING)) return;

        if (Input.GetMouseButtonDown(1) && !isCoolDown)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                show.SetActive(true);
                StartCoroutine(ShowRange_Co());
                isCoolDown = true;
                skillEffectPrefab.transform.position = hit.point;
                skillEffectPrefab.SetActive(true);
                StartCoroutine(SkillDuration_Co());
                StartCoroutine(SkillCoolDown_Co());
            }
        }
    }

    private void SkillHeal()
    {
        if (!skillType.Equals(SkillType.HEAL)) return;

        if (Input.GetMouseButtonDown(1) && !isCoolDown)
        {
            isCoolDown = true;
            Heal();
            StartCoroutine(SkillDuration_Co());
            StartCoroutine(SkillCoolDown_Co());
        }
    }


    // ��ų ���ӽð�
    IEnumerator SkillDuration_Co()
    {
        yield return new WaitForSeconds(data.duration);
        skillEffectPrefab.SetActive(false);
    }

    // ��ų ���� �� ���� ��ų �������� ��Ÿ��
    IEnumerator SkillCoolDown_Co()
    {
        yield return new WaitForSeconds(data.coolDown);
        isCoolDown = false;
    }

    IEnumerator ShowRange_Co()
    {
        yield return new WaitForSeconds(2);
        show.SetActive(false);
    }

    // Red
    IEnumerator DotDamage_Co()
    {
        for (int i = 0; i < data.dot;)
        {
            targetHp -= data.damage;
            yield return new WaitForSeconds(data.dot / data.dot);
            i++;
        }
    }

    // Blue
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

    // Green
    private void Heal()
    {
        // 찾은 오브젝트들 처리
        if (objectsWithTag.Equals(null)) return;
        foreach (GameObject obj in objectsWithTag)
        {
            if (obj.layer != layer)
                continue;
            PlayerController pc = obj.GetComponent<PlayerController>();
            pc.currentHp += healAmount;
        }
    }

    // Yellow
    private void SkillRay()
    {
        if (!skillType.Equals(SkillType.LIGHTNINGRAY)) return;

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

            StartCoroutine(ShotEffect_Co(hitPos));
        }
    }

    private IEnumerator ShotEffect_Co(Vector3 point)
    {

        line.SetPosition(0, transform.position);
        line.SetPosition(1, point);

        line.enabled = true;
        yield return new WaitForSeconds(data.duration);
        line.enabled = false;
    }
}