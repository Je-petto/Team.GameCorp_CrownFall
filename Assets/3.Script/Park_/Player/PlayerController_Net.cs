using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerController_Net : NetworkBehaviour
{
    [Header("Inspector Window")]
    public CharacterInfo T_characterInfo;
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
    [SyncVar]
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
    [SyncVar]
    public int teamCode;

    [SyncVar]
    public GameObject projection;
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("[Client] : New Client On This Server");
        hp = 0;

        if (!isLocalPlayer)
        {
            StartCoroutine(SetMapModel_Co());
        }
        else
        {
            CMDSetCID(T_characterInfo.cid);
        }

        //카메라 세팅
        SetCamera();
    }

    IEnumerator SetMapModel_Co()
    {
        yield return new WaitUntil(() => !cid.Equals(""));
        Debug.Log($"[Client] None Local Player Set Model!");
        ApplyCharactermodel(cid);
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

    public void SetCamera()
    {
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;
    }

    [ClientRpc]
    void RPCUpdateApperence(string cid)
    {
        ApplyCharactermodel(cid);
        
        CMDSetCharacterInfo(T_characterInfo.hp, T_characterInfo.speed, T_characterInfo.cid, T_characterInfo.projection);
    }

    public void ApplyCharactermodel(string cid)
    {
        Transform mesh = transform.Find("_mesh");
        if (mesh.childCount > 0)
        {
            Debug.Log($"[Client ({netId})] : 이미 메쉬 데이터가 있습니다...");
            return;
        }

        Debug.Log("Cid Change : new Character model set.");
        CharacterInfo info = PlayerSpawner.I.GetCharacterInfo(cid);

        if (info == null)
        {
            Debug.Log("Character Info is null...");
            return;
        }

        GameObject model = info.inGameModel;
        GameObject characterModel = Instantiate(model, mesh);

        this.data = info;

        // CMDSetCharacterInfo(T_characterInfo.hp, T_characterInfo.speed, T_characterInfo.cid, T_characterInfo.projection);
        animator = GetComponentInChildren<Animator>();
        InitComponents();
    }

    [Command]
    void CMDSetCharacterInfo(int currentHp, float moveSpeed, string cid, GameObject projection)
    {
        Debug.Log("CMD Set Character information");
        Debug.Log($"hp : {currentHp}, speed : {moveSpeed}, cid : {cid}");

        this.hp = currentHp;
        this.speed = moveSpeed;
        this.projection = projection;
    }

    void InitComponents()
    {
        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);

        if (lineRenderer != null) lineRenderer.enabled = false;

        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.detectCommand = new DetectCommand(this, new(this));
        inputHandler.attackCommand = new AttackCommand(this, new(this));
    }

    #region Network Part
    [Command]
    public void AttackBasic()
    {
        Debug.Log("Attack Basic!!");

        GameObject attackOrb = Instantiate(projection, attackPoint.position, Quaternion.identity);
        Debug.Log("network spawn ready...");
        NetworkServer.Spawn(attackOrb);
    }
    #endregion
}