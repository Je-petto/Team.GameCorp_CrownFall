using System.Collections;
using Cinemachine;
using Mirror;
using UnityEngine;
using UnityEngine.Events;


public enum LifeState { ALIVE, DEATH }

public class PlayerStat
{
    public int hp;
    public float moveSpeed;

    public PlayerStat(int hp, float moveSpeed)
    {
        this.hp = hp;
        this.moveSpeed = moveSpeed;
    }
}

public class PlayerController : NetworkBehaviour
{
    #region Test
    [Header("Test")]
    public CharacterInfo T_data;
    #endregion

    #region Event
    
    public UnityAction<float> OnChangedHp;
    #endregion

    #region PlayerStat
    [Header("Player Stat")]

    public PlayerStat currentStat;

    public CharacterInfo data;
    public LifeState pState;
    
    [Header("Respawn Settings")]
    public float respawnTime = 5f;
    #endregion

    #region Components
    [Header("Private")]
    public Rigidbody rb;
    public Animator animator;
    public Transform attackPoint;
    public LineRenderer lineRenderer;
    public PlayerStateMachine stateMachine;
    public PlayerInputHandler inputHandler;
    public EffectHandler effectHandler;
    #endregion

    #region Misc
    public int teamCode = 0;
    public Vector3 targetPoint;
    #endregion

    void Start()
    {
        Debug.Log("Player Init Start!");
        pState = LifeState.ALIVE;
        teamCode = 0;

        StartCoroutine(SetCharacter_Co());
    }

    IEnumerator SetCharacter_Co()
    {
        // Debug.Log("Player Character Setting...");
        // yield return new WaitUntil(() => InGameSession.isInit);

        Debug.Log("InGameSession Complete.");
        yield return new WaitUntil(() => PlayerSpawner.I != null);

        Debug.Log($"{InGameSession.characterId}...");
        Debug.Log("PlayerSpawner Complete.");

        // CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(InGameSession.characterId);
        CharacterInfo charData = T_data;

        if (charData != null)
            Debug.Log("char data is not null.");
        else
            Debug.LogWarning("char data is null.");

        data = charData;

        currentStat = new(data.hp, data.speed);

        Instantiate(data.model, Vector3.zero, Quaternion.Euler(Vector3.zero), transform.Find("_mesh"));

        StartCoroutine(InitComponents_Co());
    }

    IEnumerator InitComponents_Co()
    {
        yield return new WaitForEndOfFrame();
        Debug.Log("InitComponents_Co Ing...");

        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);

        if (inputHandler == null)
        {
            Debug.Log("inputHandler still null, retrying...");
            inputHandler = GetComponent<PlayerInputHandler>();
        }

        if (inputHandler == null)
        {
            Debug.Log("inputHandler is permanently null!");
            yield break;
        }

        if (rb == null) Debug.Log("rb is null");
        if (stateMachine == null) Debug.Log("stateMachine is null");
        if (lineRenderer == null) Debug.Log("lineRenderer is null");

        if (lineRenderer != null) lineRenderer.enabled = false;

        animator = GetComponentInChildren<Animator>();
        // GetComponent<NetworkAnimator>().animator = animator;                //네트워크 데이터 동기화 추가
        
        if (animator == null) Debug.Log("animator is null");
        animator.runtimeAnimatorController = data.inGameAnimator;

        // Cinemachine 카메라 설정
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;

        // EffectHandler 초기화
        effectHandler = new EffectHandler(this);

        yield return new WaitUntil(() => inputHandler != null);

        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackNonTargeting(this));
        inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));
        inputHandler.deathCommand = new DeathCommand(this, new DeadState(this));

        SkillData sd = data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        if (sd == null) Debug.Log("sd == null");

        ISkillAction skillAction = SkillFactory.CreateSkillAction(this, sd);
        if (skillAction == null) Debug.Log("skillAction == null");

        inputHandler.skillCastCommand = new SkillCastCommand(this, skillAction);
    }
    
    public void RaiseOnChangeHp()
    {
        Debug.Log("데미지 적용!");
        OnChangedHp?.Invoke(currentStat.hp / data.hp);
    }

    public void Die()
    {
        stateMachine.ChangeState(new DeadState(this));
    }
}