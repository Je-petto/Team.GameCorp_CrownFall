using System;
using System.Collections;
using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

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

    [SyncVar(hook = "ChangeHp")]
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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("▶ LocalPlayer Init");
        if (!isLocalPlayer)
        {
            SetMapModel_Co();
        }
        else
        {
            CMDSetCID(T_characterInfo.cid);
        }
        //카메라 세팅
        SetCamera();
    }

    void SetMapModel_Co()
    {
        Debug.Log($"[Client] None Local Player Set Model!");
        // ApplyCharactermodel(cid);
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
        GameObject model = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid).inGameModel;
        Transform mesh = transform.Find("_mesh");

        if (model == null)
        {
            Debug.Log($"model is null..");
            return;
        }

        if (mesh.childCount > 0)
        {
            Debug.Log($"[Client ({netId})] : 이미 메쉬 데이터가 있습니다...");
            return;
        }

        Instantiate(model, mesh);
        InitComponents();
    }

    public void SetCamera()
    {
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;
    }


    [Command]
    public void CMDSetCharacterInfo(int hp, float speed)
    {
        this.hp = hp;
        this.speed = speed;
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
    public void AttackBasic()
    {
        Debug.Log("Attack Basic!!");
        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid);
        GameObject projection = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid).projection;

        if (projection == null)
        {
            Debug.Log("projection is null...");
        }

        GameObject attackOrb = Instantiate(projection);
        Debug.Log("network spawn ready...");
        NetworkServer.Spawn(attackOrb);

        attackOrb.GetComponent<AttackObject>().SetAttack(this, info.attack, targetPoint);
    }

    public UnityAction<float> OnChangeHp;

    void ChangeHp(int hp)
    {
        this.hp = hp;

        OnChangeHp(hp / data.hp);
    }
    #endregion
}