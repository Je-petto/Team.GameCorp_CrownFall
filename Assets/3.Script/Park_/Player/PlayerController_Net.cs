using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerController_Net : NetworkBehaviour
{
    [Header("Inspector Window")]
    public CharacterInfo T_characterInfo;
    public CharacterInfo data;

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

    [SyncVar]
    public string cid;              //자신의 캐릭터 번호.

    public Vector3 targetPoint;

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("[Client] : New Client On This Server");

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

        animator = GetComponentInChildren<Animator>();

        InitComponents();
    }

    void InitComponents()
    {
        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);

        if (lineRenderer != null) lineRenderer.enabled = false;

        inputHandler.moveCommand = new MoveCommand(this);
    }
}