using System.Collections;
using Cinemachine;
using Mirror;
using UnityEngine;


public enum LifeState { ALIVE, DEATH }

public class PlayerStat
{
    public int hp;
    public float moveSpeed;
}

public class PlayerController : NetworkBehaviour
{
    #region PlayerStat
    public int currentHp;
    public CharacterInfo data;
    public LifeState pState;

    [Header("Respawn Settings")]
    public float respawnTime = 5f;
    #endregion

    #region [HideInspector]
    public Rigidbody rb;
    public Animator animator;
    public Transform attackPoint;
    public LineRenderer lineRenderer;

    public PlayerStateMachine stateMachine;
    public PlayerInputHandler inputHandler;
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
        Debug.Log("Player Init Start!");
        pState = LifeState.ALIVE;
        teamData = new(TeamType.RED);

        StartCoroutine(SetCharacter_Co());
    }

    IEnumerator SetCharacter_Co()
    {
        Debug.Log("Player Character Setting...");
        yield return new WaitUntil(() => InGameSession.isInit);

        Debug.Log("InGameSession Complete.");
        yield return new WaitUntil(() => PlayerSpawner.I != null);

        Debug.Log($"{InGameSession.characterId}...");
        
        Debug.Log("PlayerSpawner Complete.");
        CharacterInfo data = PlayerSpawner.I.GetCharacterInfo(InGameSession.characterId);

        if (data != null)
        {
            Debug.Log("char data is not null.");
        }
        else
        {
            Debug.Log("char data is null.");   
        }

        this.data = data;
        currentHp = data.hp;

        Instantiate(data.model, Vector3.zero, Quaternion.Euler(Vector3.zero), transform.Find("_mesh"));

        // 로컬 테스트용.
        StartCoroutine(InitComponents_Co());
    }

    IEnumerator InitComponents_Co()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("InitComponents_Co Ing...");
        lineRenderer.enabled = false;

        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);

        if (rb == null)
        {
            Debug.Log("rb is null\n");
        }

        if (stateMachine == null)
        {
            Debug.Log("stateMachine is null\n");
        }

        if (inputHandler == null)
        {
            Debug.Log("inputHandler is null\n");
        }

        if (lineRenderer == null)
        {
            Debug.Log("lineRenderer is null\n");
        }



        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = data.inGameAnimator;

        // 카메라 => Cinemachine 세팅
        FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;

        // 동적 생성
        effectHandler = new EffectHandler(this);
        yield return new WaitUntil(() => inputHandler != null);

        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackNonTargeting(this));            // Backstep

        // inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackIK(this));                         // IK
        inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));
        inputHandler.deathCommand = new DeathCommand(this, new DeadState(this));

        // SkillData sd = data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        // inputHandler.skillCastCommand = new SkillCastCommand(this, SkillFactory.CreateSkillAction(this, sd));


    }

    //추가한 부분 시작
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        if (currentHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        stateMachine.ChangeState(new DeadState(this));
    }

    void OnDrawGizmos()
    {
        if (data == null) return;

        //공격 범위 드로우
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, data.attackableRange);
    }
}