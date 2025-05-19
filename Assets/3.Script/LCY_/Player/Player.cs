using System.Collections;

using UnityEngine;

using CustomInspector;

public enum STATE
{
    NONE,
    PERSIST,
    SLOW,
}

public class Player : MonoBehaviour
{
    [ReadOnly] public int maxHp = 1000;
    [ReadOnly] public int curHp;
    [ReadOnly] public float moveSpeed;
    [ReadOnly] public float attackSpeed;
    [ReadOnly] public STATE state = STATE.NONE;

    [SerializeField, Tooltip("피해 지속시간")] private int persistTime;
    [SerializeField, Tooltip("스킬 발동 딜레이")] private float delayTime = 2f;
    [SerializeField, Tooltip("스킬 쿨타임")] private float coolTime = 10f;
    [SerializeField] private GameObject temppt;
    [SerializeField] private GameObject tempat;

    private GameObject ptc;
    private GameObject redskill;

    private bool coolTime_ = false;

    private void Awake()
    {
        curHp = maxHp;
        moveSpeed = 30f;
        attackSpeed = 30f;
        Initialize();
    }

    private void Initialize()
    {
        GameObject particle = new GameObject();
        GameObject skill = new GameObject();
        particle.name = "ParticlePool";
        skill.name = "Skill";
        ptc = Instantiate(temppt, particle.transform);
        redskill = Instantiate(tempat, skill.transform);
        ptc.SetActive(false);
        redskill.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButton(1) && !coolTime_)
        {
            coolTime_ = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                ptc.transform.position = hit.point;
                redskill.transform.position = hit.point;
                ptc.SetActive(true);
                StartCoroutine(ShowRange_Co());
                StartCoroutine(CoolTime_Co());
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedSkill")
        {
            curHp -= 900;
            state = STATE.PERSIST;
            if (state == STATE.PERSIST) ;
            StartCoroutine(PersistDamage_Co());
        }

        if (other.tag == "BlueSkill")
        {
            curHp -= 900;
            state = STATE.SLOW;
            if (state == STATE.SLOW)
                StartCoroutine(SlowPlayer_Co());
        }
    }

    IEnumerator ShowRange_Co()
    {
        yield return new WaitForSeconds(2);
        ptc.SetActive(false);
        redskill.SetActive(true);
    }

    IEnumerator CoolTime_Co()
    {
        yield return new WaitForSeconds(coolTime);
        redskill.SetActive(false);
        coolTime_ = false;
    }

    IEnumerator PersistDamage_Co()
    {
        for (int i = 0; i < persistTime;)
        {
            curHp -= 10;
            yield return new WaitForSeconds(1);
            i++;
        }
        state = STATE.NONE;
    }

    IEnumerator SlowPlayer_Co()
    {
        float tempMove = moveSpeed;
        float tempAttack = attackSpeed;

        moveSpeed *= 0.15f;
        attackSpeed *= 0.15f;
        yield return new WaitForSeconds(6);
        state = STATE.NONE;
        moveSpeed = tempMove;
        attackSpeed = tempAttack;
    }
}