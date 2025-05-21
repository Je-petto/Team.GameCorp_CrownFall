using System.Collections;
using Cinemachine;
using UnityEngine;

public enum TeamType
{
    Red,
    Blue
}

public class TeamComponent
{
    public TeamType type;

    public TeamComponent(TeamType type)
    {
        this.type = type;
    }

    public bool IsEnemy(TeamComponent component)
    {
        return type != component.type;
    }
}

public class PlayerController : MonoBehaviour
{
    #region PlayerStat
    public int currentHp;
    public CharacterInfo data;
    #endregion

    #region [HideInspector]
    public Rigidbody rb;
    public Animator animator;
    public Transform attackPoint;
    public LineRenderer lineRenderer;

    public PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;
    public EffectHandler effectHandler;
    #endregion

    #region Test
    public TeamComponent teamData = null;
    public PlayerController target = null;                          //제거 예정
    public Vector3 targetPoint;
    // private 
    #endregion

    public HumanBodyBones bone;

    void Start()
    {
        //test Code
        teamData = new(TeamType.Red);
    }

    public void SetCharacter(CharacterInfo data)
    {
        this.data = data;
        currentHp = data.hp;
        Instantiate(data.model, transform.Find("_mesh"));

        // 로컬 테스트용.
        StartCoroutine(InitComponents_Co());
    }

    IEnumerator InitComponents_Co()
    {
        yield return new WaitForEndOfFrame();

        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);

        lineRenderer.enabled = false;

        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = data.inGameAnimator;

        // 카메라 => Cinemachine 세팅
        FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;

        // 동적 생성
        effectHandler = new EffectHandler(this);
        yield return new WaitUntil(() => inputHandler != null);

        inputHandler.moveCommand = new MoveCommand(this);

        // inputHandler.attackCommand = new AttackCommand(this. new PlayerAttack(this));
        // inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackNonTargeting(this));            // Backstep
        inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackIK(this));                         // IK
        inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));

        SkillData sd = data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        inputHandler.skillCastCommand = new SkillCastCommand(this, SkillFactory.CreateSkillAction(this, sd));
    }

    void OnDrawGizmos()
    {
        if (data == null) return;

        //공격 범위 드로우
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, data.attackableRange);
    }

    #region Inspector Test
    void LateUpdate()
    {

    }
    #endregion
}