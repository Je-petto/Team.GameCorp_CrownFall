using System;
using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;


public enum LifeState { ALIVE, DEATH }

[Serializable]
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

public class PlayerController : MonoBehaviour // NetworkBehaviour
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
    public AnimationHandler animationHandler;
    #endregion

    #region Misc
    public int teamCode = 0;
    public Vector3 targetPoint;
    #endregion

    void Start()
    {
        // if (!isLocalPlayer) return;

        Debug.Log("Player Init Start!");
        pState = LifeState.ALIVE;
        // CmdRequestMyUserData(InGameSession.uid); 
        EditorTest();   
    }

    #region Editor
    void EditorTest()
    {
        // if (teamCode == 1) return;
        StartCoroutine(SpawnCharacter(T_data.cid));
    }

    #endregion

    // #region Client
    // [Command]
    // private void CmdRequestMyUserData(string uid)
    // {
    //     var userData = ((InGameNetworkManager)NetworkManager.singleton).GetUser(uid);

    //     if (userData != null)
    //     {
    //         TargetReceiveUserData(connectionToClient, userData);
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"[Server] 유저 정보 없음: {uid}");
    //     }
    // }

    // [TargetRpc]
    // private void TargetReceiveUserData(NetworkConnection target, UserAuth data)
    // {
    //     Debug.Log($"[Client] 내 유저 정보 수신: {data.nickname} / {data.c_id} / 팀: {data.teamCode}");

    //     // 캐릭터 데이터 로드 및 생성
    //     StartCoroutine(SpawnCharacter(data.c_id));
    // }
    // #endregion
    
    private IEnumerator SpawnCharacter(string cid)
    {
        yield return new WaitUntil(() => PlayerSpawner.I != null);

        CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(cid);

        Debug.Log($"[Client] 캐릭터 데이터 처리 : {charData}");
        if (charData == null)
        {
            Debug.LogError($"[Client] 캐릭터 정보 없음: {cid}");
            yield break;
        }

        // 캐릭터 모델 생성
        Instantiate(charData.inGameModel, Vector3.zero, Quaternion.identity, transform.Find("_mesh"));

        Debug.Log($"[Client] 캐릭터 '{cid}' instantiate Complete.");

        data = charData;
        currentStat = new(data.hp, data.speed);

        Debug.Log($"[Client] 캐릭터 '{currentStat.hp} , {currentStat.moveSpeed}' set Complete");

        StartCoroutine(InitComponents_Co());
    }

    IEnumerator InitComponents_Co()
    {
        Debug.Log("InitComponents_Co Ing...");
        yield return new WaitForEndOfFrame();

        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);
        TryGetComponent(out animationHandler);

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
        
        if (animator == null) Debug.Log("animator is null");

        Debug.Log(data.inGameAnimator + $"{data.inGameAnimator}");
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
        // if (!isLocalPlayer) return;
        Debug.Log("데미지 적용!");
        OnChangedHp?.Invoke((float)currentStat.hp / data.hp);
    }

    public void Die()
    {
        // if (!isLocalPlayer) return;
        stateMachine.ChangeState(new DeadState(this));
    }
}