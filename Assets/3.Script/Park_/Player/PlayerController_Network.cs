using System.Collections;
using Cinemachine;
using Mirror;
using UnityEngine;

public class PlayerController_Network : NetworkBehaviour
{
    #region Network

    [SyncVar] public string uid;
    [SyncVar] public string characterId;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        Debug.Log("[Client] OnStartLocalPlayer");

        // 클라이언트에서 받은 데이터를 커맨드로 서버에 전송
        CmdSendInitData(InGameSession.uid, InGameSession.characterId);
    }

    [Command]
    void CmdSendInitData(string uid, string cid)
    {
        this.uid = uid;
        this.characterId = cid;

        Debug.Log($"[Server] Received init data: UID={uid}, CID={cid}");

        // 여기에 플레이어 초기화 로직 가능 (팀 설정, 외형 로딩 등)
        // SetCharacter(.......);
    }
    #endregion


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
        //test Code
        StartCoroutine(SetCharacter());
        teamData = new(TeamType.RED);
    }

    IEnumerator SetCharacter()
    {
        yield return new WaitUntil(() => PlayerSpawner.I != null);
        CharacterInfo data = PlayerSpawner.I.GetCharacterInfo();

        this.data = data;
        currentHp = data.hp;
        Instantiate(data.model, Vector3.zero, Quaternion.Euler(Vector3.zero), transform.Find("_mesh"));

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
        // effectHandler = new EffectHandler(this);
        // yield return new WaitUntil(() => inputHandler != null);

        // inputHandler.moveCommand = new MoveCommand(this);

        // // inputHandler.attackCommand = new AttackCommand(this. new PlayerAttack(this));
        // // inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackNonTargeting(this));            // Backstep
        // inputHandler.attackCommand = new AttackCommand(this, new PlayerAttackIK(this));                         // IK
        // inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));

        // SkillData sd = data.skillSet.Find(s => !s.type.Equals(SkillType.NONE));
        // inputHandler.skillCastCommand = new SkillCastCommand(this, SkillFactory.CreateSkillAction(this, sd));
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