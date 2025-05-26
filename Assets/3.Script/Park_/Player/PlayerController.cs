using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.VisualScripting;
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

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public string uid;

    [SyncVar]
    public string nickname;

    [SyncVar]
    public string c_id;

    [SyncVar]
    public int teamCode;

    #region Test
    [Header("Test")]
    public CharacterInfo T_data;
    #endregion
    #region Event

    [SyncVar(hook = nameof(OnSyncHpChanged))]
    public int syncedHp;

    [SyncVar]
    public string characterId; // cid 역할

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
    public Vector3 targetPoint;
    #endregion

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(SpawnCharacter(data.cid));
    }

    void Start()
    {
        Debug.Log("Player Init Start!");
        pState = LifeState.ALIVE;

        syncedHp = currentStat.hp; // UI 동기화를 위해
        CmdRequestMyUserData(InGameSession.uid);
        // EditorTest();   
    }

    #region Editor
    void EditorTest()
    {
        // if (teamCode == 1) return;
        StartCoroutine(SpawnCharacter(T_data.cid));
    }

    #endregion

    #region Client
    [Command]
    private void CmdRequestMyUserData(string uid)
    {
        var userData = ((InGameNetworkManager)NetworkManager.singleton).GetUser(uid);
        if (userData != null)
        {
            characterId = userData.c_id; // ✅ 여기에 추가
            TargetReceiveUserData(connectionToClient, userData);
        }
    }

    [TargetRpc]
    private void TargetReceiveUserData(NetworkConnection target, UserAuth data)
    {
        Debug.Log($"[Client] 내 유저 정보 수신: {data.nickname} / {data.c_id} / 팀: {data.teamCode}");

        // 서버에 캐릭터 모델 스폰 요청
        CmdSpawnCharacterModel(data.c_id);

        // 클라이언트 로컬에서 데이터 세팅 등 추가 처리
        StartCoroutine(SpawnCharacter(data.c_id));
    }

    [Command]
    private void CmdSpawnCharacterModel(string cid)
    {
        StartCoroutine(WaitAndSpawn(cid));
    }
    #endregion

    private IEnumerator WaitAndSpawn(string cid)
    {
        while (PlayerSpawner.I == null)
        {
            Debug.Log("[Server] PlayerSpawner.I is null, 대기 중...");
            yield return null; // 다음 프레임까지 대기
        }

        Debug.Log("[Server] PlayerSpawner.I 초기화 완료, SpawnCharacterModel 실행");
        SpawnCharacterModel(cid);
    }

    [Server]
    public void SpawnCharacterModel(string cid)
    {
        CharacterInfo charInfo = PlayerSpawner.I.GetCharacterInfo(cid);

        if (PlayerSpawner.I == null)
        {
            Debug.LogWarning($"[Server] PlayerSpawner.I Not Found: ");
        }

        if (charInfo == null)
        {
            Debug.LogWarning($"[Server] CharacterInfo Not Found: {cid}");
            return;
        }

        Debug.Log($"[Server] : charInfo {charInfo}");

        GameObject modelObj = Instantiate(charInfo.inGameModel, transform.Find("_mesh"));

        // 소유권을 명확하게 할당
        NetworkServer.Spawn(modelObj, connectionToClient);

        Debug.Log($"[Server] 모델 Spawn 완료: {cid} (Owner: {connectionToClient.connectionId})");
    }

    // [Server]
    // public void SpawnCharacterModel(string cid)
    // {
    //     CharacterInfo charInfo = PlayerSpawner.I.GetCharacterInfo(cid);
    //     if (charInfo == null)
    //     {
    //         Debug.LogError($"[Server] CharacterInfo Not Found: {cid}");
    //         return;
    //     }

    //     GameObject modelObj = Instantiate(charInfo.inGameModel);
    //     modelObj.transform.SetParent(transform.Find("_mesh"), false);
    //     NetworkServer.Spawn(modelObj, connectionToClient);
    // }


    private IEnumerator SpawnCharacter(string cid)
    {
        yield return new WaitUntil(() => PlayerSpawner.I != null);

        CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(cid);

        if (charData == null)
        {
            Debug.LogError($"[Client] 캐릭터 정보 없음: {cid}");
            yield break;
        }

        Transform meshParent = transform.Find("_mesh");
        if (meshParent == null)
        {
            Debug.LogError("[Client] _mesh 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        // 이미 모델이 있으면 삭제 또는 스킵 (중복 방지)
        foreach (Transform child in meshParent)
        {
            Destroy(child.gameObject);
        }

        GameObject model = Instantiate(charData.inGameModel, Vector3.zero, Quaternion.identity, meshParent);
        model.SetActive(true);

        Debug.Log($"[Client] 캐릭터 '{cid}' 모델 인스턴스 생성 완료");

        data = charData;
        currentStat = new PlayerStat(data.hp, data.speed);

        StartCoroutine(InitComponents_Co());
    }

    // private IEnumerator SpawnCharacter(string cid)
    // {
    //     yield return new WaitUntil(() => PlayerSpawner.I != null);

    //     CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(cid);

    //     Debug.Log($"[Client] 캐릭터 데이터 처리 : {charData}");
    //     if (charData == null)
    //     {
    //         Debug.LogError($"[Client] 캐릭터 정보 없음: {cid}");
    //         yield break;
    //     }

    //     // 캐릭터 모델 생성
    //     Instantiate(charData.inGameModel, Vector3.zero, Quaternion.identity, transform.Find("_mesh"));

    //     Debug.Log($"[Client] 캐릭터 '{cid}' instantiate Complete.");

    //     data = charData;
    //     currentStat = new(data.hp, data.speed);

    //     Debug.Log($"[Client] 캐릭터 '{currentStat.hp} , {currentStat.moveSpeed}' set Complete");

    //     StartCoroutine(InitComponents_Co());
    // }

    IEnumerator InitComponents_Co()
    {
        if (!isLocalPlayer) yield break;

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
        if (!isLocalPlayer) return;
        Debug.Log("데미지 적용!");
        OnChangedHp?.Invoke((float)currentStat.hp / data.hp);
    }

    public void Die()
    {
        if (!isLocalPlayer) return;
        stateMachine.ChangeState(new DeadState(this));
    }


    [Server]
    public void TakeDamage(int damage)
    {
        currentStat.hp -= damage;
        currentStat.hp = Mathf.Max(0, currentStat.hp);

        syncedHp = currentStat.hp; // 클라이언트 UI 동기화
    }

    private void OnSyncHpChanged(int oldValue, int newValue)
    {
        if (!isLocalPlayer) return;

        float percent = (float)newValue / data.hp;
        Debug.Log($"[Client] HP UI 업데이트: {percent * 100}%");

        RaiseOnChangeHp(); // 기존 이벤트 구조 활용
    }
    

    [Command]
    public void CmdSpawnModel(string cid)
    {
        CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(cid);

        if (charData == null)
        {
            Debug.LogError($"[Server] 캐릭터 정보 없음: {cid}");
            return;
        }

        GameObject model = Instantiate(charData.inGameModel, transform.position, Quaternion.identity, transform.Find("_mesh"));
        NetworkServer.Spawn(model, connectionToClient); // 이걸 통해 해당 유저의 소유로 등록됨
    }

    [Command]
    public void CmdCastSkill(string skillId, Vector3 point,  SkillData data)
    {
        GameObject skillObj = Instantiate(data.prefab, point, Quaternion.identity);

        SkillEffectController controller = skillObj.GetComponent<SkillEffectController>();
        controller.SetProps(this, data);

        NetworkServer.Spawn(skillObj); // 다른 클라이언트에게도 보이게

        RpcActivateSkill(skillObj, data.duration);
    }

    [ClientRpc]
    void RpcActivateSkill(GameObject skillObj, float duration)
    {
        StartCoroutine(SkillEffectRoutine(skillObj, duration));
    }

    IEnumerator SkillEffectRoutine(GameObject skillObj, float duration)
    {
        skillObj.SetActive(true);
        yield return new WaitForSeconds(duration);
        skillObj.SetActive(false);
    }
}