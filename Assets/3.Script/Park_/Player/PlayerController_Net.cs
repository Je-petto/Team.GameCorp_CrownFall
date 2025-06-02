using System.Collections;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerController_Net : NetworkBehaviour
{
    [Header("Inspector Window")]
    public CharacterInfo data;

    #region Sync Value
    [Header("Field")]
    [SyncVar]
    public string cid;              // 캐릭터 고유 번호.
    [SyncVar]
    public string characterNickName;    // 캐릭터 이름
    [SyncVar]
    public string characterName;    // 캐릭터 이름
    [SyncVar]
    public string description;      // 캐릭터 설명

    [SyncVar(hook = nameof(ChangeHp))]
    public int hp;                  // 체력

    [SyncVar]
    public int attack;              // 공격력

    [SyncVar]
    public int defense;

    [SyncVar]
    public float speed;               // 이동 속도

    [SyncVar]
    public float attackableRange;   // 공격 가능 범위

    [SyncVar]
    public float attackInterval;    // 공격 주기.

    [SyncVar(hook = nameof(OnInitTeam))]
    public int teamCode = 0;
    public float rotateSpeed;                       // 회전 속도
    #endregion

    #region Components
    [Header("Components")]
    public Rigidbody rb;
    public Animator animator;
    public Transform attackPoint;
    public LineRenderer lineRenderer;
    public PlayerStateMachine stateMachine;
    public PlayerInputHandler inputHandler;
    public EffectHandler effectHandler;
    public AnimationHandler animationHandler;
    #endregion
    public Vector3 targetPoint;

    LocalPlayerSetter setter;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        setter = FindAnyObjectByType<LocalPlayerSetter>();
        data = FindAnyObjectByType<LocalPlayerSetter>().info;

        if (!isLocalPlayer)
        {
            SetMapModel_Co();
        }
        else
        {
            string id = data.cid;
            CMDSetCID(id);
        }
        //카메라 세팅
        SetCamera();
    }

    void SetMapModel_Co()
    {
        Debug.Log($"[Client] None Local Player Set Model! {cid}");
        RPCUpdateApperence(cid);
    }

    void Start()
    {
        Debug.Log("[Client] : Player Start!");
    }

    [Command]
    public void CMDSetCID(string cid)
    {
        this.cid = cid;

        RPCUpdateApperence(cid);
    }

    [ClientRpc]
    void RPCUpdateApperence(string cid)
    {
        //서버에서 가져오기
        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid);
        this.data = info;

        GameObject model = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid).inGameModel;
        Transform mesh = transform.Find("_mesh");

        if (model == null)
        {
            Debug.Log($"model is null..");
            return;
        }

        if (mesh.childCount > 0)
        {
            Debug.Log($"[Client ({netId})] : exist model data.");
            return;
        }

        int code = FindAnyObjectByType<LocalPlayerSetter>().teamCode;
        Instantiate(model, mesh);
        InitComponents();

        CMDSetCharacterInfo(info.hp, info.speed, info.attackableRange, code);
    }

    public void SetCamera()
    {
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;
    }

    [Command]
    public void CMDSetCharacterInfo(int hp, float speed, float attackableRange, int teamCode)
    {
        this.hp = hp;
        this.speed = speed;
        this.attackableRange = attackableRange;
        this.teamCode = teamCode;
    }

    void InitComponents()
    {
        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);
        TryGetComponent(out effectHandler);

        animator = GetComponentInChildren<Animator>();

        if (lineRenderer != null) lineRenderer.enabled = false;

        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.detectCommand = new DetectCommand(this, new(this));
        inputHandler.attackCommand = new AttackCommand(this, new(this));
    }

    #region Network Part
    [Command]
    public void CMDAttackBasic(Vector3 targetPoint)
    {
        Debug.Log("Attack Basic!!");
        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid);
        GameObject projection = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid).projection;

        if (projection == null)
        {
            Debug.Log("projection is null...");
        }

        GameObject attackOrb = Instantiate(projection, transform.position, Quaternion.identity);
        Debug.Log("network spawn ready...");
        NetworkServer.Spawn(attackOrb);

        Debug.Log($" Set Target : {targetPoint}");
        attackOrb.GetComponent<AttackObject>().SetAttack(this, info.attack, targetPoint);
    }

    void ChangeHp(int preVal, int newVal)
    {
        OnChangeCurrentHpBar((float)newVal / data.hp);
    }
    #endregion


    void Update()
    {
        if (inputHandler == null) return;
        if (!isLocalPlayer) return;
        inputHandler.InputUpdate();
    }

    void FixedUpdate()
    {
        if (inputHandler == null) return;
        if (!isLocalPlayer) return;
        inputHandler.InputFixedUpdate();
    }


    #region UI
    [SerializeField] Image teamColorHpbar;

    void OnInitTeam(int oldVal, int newVal)
    {
        Debug.Log("TeamCode Change!!");
        if (teamColorHpbar == null)
        {
            Debug.Log("teamColorHpbar is null...");
            return;
        }
        teamColorHpbar.color = newVal == 0 ? Color.red : Color.blue;
    }

    void OnChangeCurrentHpBar(float percent)
    {
        teamColorHpbar.fillAmount = percent;
    }
    #endregion
}