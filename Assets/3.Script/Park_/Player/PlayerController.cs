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

public class PlayerController : NetworkBehaviour
{
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



    [SyncVar(hook = nameof(OnMeshIndexChanged))]
    public int meshIndex = 0;

    public int teamCode;
    #endregion

    void Start()
    {

        Debug.Log("Player Init Start!");
        pState = LifeState.ALIVE;

        syncedHp = currentStat.hp; // UI 동기화를 위해
        meshIndex = 0;

        if (!isLocalPlayer) return;
        CmdRequestMyUserData(InGameSession.uid);
        // EditorTest();   
    }

    private void OnMeshIndexChanged(int oldValue, int newValue)
    {
        Transform meshTransform = transform.Find("_mesh");

        if (meshTransform == null)
        {
            Debug.LogWarning("_mesh is null...");
            return;
        }

        if (newValue < 0 || newValue >= meshTransform.childCount)
        {
            Debug.LogWarning("meshIndex over.");
            return;
        }

        Transform targetChild = meshTransform.GetChild(newValue);
        targetChild.localScale = Vector3.one;  // (1,1,1)
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
        var userData = ((InGameNetworkManager)NetworkManager.singleton).GetUser(connectionToClient, uid);
        if (userData != null)
        {
            characterId = userData.c_id; // ✅ 여기에 추가
            switch (userData.c_id)
            {
                case "P-000":
                    {
                        meshIndex = 0;
                    break;
                }
                case "P-001":
                    {
                        meshIndex = 1;
                    break;
                }
                case "P-002":
                    {
                        meshIndex = 2;
                    break;
                }case "P-003":
                {
                    meshIndex = 3;
                    break;
                }
            }
            StartCoroutine(SpawnCharacter(userData.c_id));
        }
    }
    #endregion


    [TargetRpc]
    public void RecieveCharacterData(UserAuth user)
    {
        //서버로 부터 메시를 매핑하라고 받은 데이터.
        if (!isLocalPlayer) return;         //자기가 보낸 걸 받은 것이면 무시한다.
        teamCode = user.teamCode;
        StartCoroutine(SpawnCharacter(user.c_id));
    }


    private IEnumerator SpawnCharacter(string cid)
    {
        yield return new WaitUntil(() => PlayerSpawner.I != null);

        CharacterInfo charData = PlayerSpawner.I.GetCharacterInfo(cid);

        if (charData == null)
        {
            Debug.LogError($"[Client] None Character : {cid}");
            yield break;
        }

        Transform meshParent = transform.Find("_mesh");
        if (meshParent == null)
        {
            Debug.LogError("[Client] _mesh 오브젝트를 찾을 수 없습니다.");
            yield break;
        }

        // GameObject model = Instantiate(charData.inGameModel, Vector3.zero, Quaternion.identity, meshParent);
        // model.SetActive(true);

        Debug.Log($"[Client] Character '{cid}' instantiate complete");

        data = charData;
        currentStat = new PlayerStat(data.hp, data.speed);

        StartCoroutine(InitComponents_Co());
    }

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

        GetComponentInChildren<PlayerUIController>().SetUI();
        StartCoroutine(SetIngameUI_Co());
    }

    IEnumerator SetIngameUI_Co()
    {
        yield return new WaitUntil(() => data != null);
        UIManager.I.SetPlayerController(this);
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
    public void CmdCastSkill(string skillId, Vector3 point, SkillData data)
    {
        GameObject skillObj = Instantiate(data.prefab, point, Quaternion.identity);

        SkillEffectController controller = skillObj.GetComponent<SkillEffectController>();
        controller.SetProps(this, data);

        NetworkServer.Spawn(skillObj);                      // 다른 클라이언트에게도 보이게

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

    [Command]
    public void CmdShoot(Vector3 targetPoint)
    {
        
    }
}