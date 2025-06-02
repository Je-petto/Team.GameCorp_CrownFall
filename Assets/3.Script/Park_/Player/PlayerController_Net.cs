using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController_Net : NetworkBehaviour
{
    [Header("Inspector Window")]
    public CharacterInfo data;

    #region Sync Value
    [Header("Field")]

    [SyncVar(hook = nameof(OnCidChanged))]
    public string cid;

    [SyncVar]
    public string characterNickName;    // 캐릭터 이름
    [SyncVar]
    public string characterName;    // 캐릭터 이름
    [SyncVar]
    public string description;      // 캐릭터 설명

    [SyncVar]
    public int fullHp;


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
    public int teamCode = -1;
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
    public AnimationHandler animationHandler;
    #endregion
    public Vector3 targetPoint;

    public override void OnStartClient()
    {
        Debug.Log("[Client] client Start!");
        base.OnStartClient();
        if (isLocalPlayer)
        {
            data = FindAnyObjectByType<LocalPlayerSetter>().info;
            CMDSetCID(data.cid);
            SetCamera();
        }
    }

    [Command]
    public void CMDSetCID(string cid)
    {
        this.cid = cid;
    }

    void OnCidChanged(string oldCid, string newCid)
    {
        Debug.Log($"[Client] cid changed → {newCid}");

        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == newCid);
        if (info == null || info.inGameModel == null) return;

        data = info;

        Transform mesh = transform.Find("_mesh");
        if (mesh.childCount > 0) return;

        Instantiate(info.inGameModel, mesh);
        InitComponents(); // 모델 생성을 기반으로 컴포넌트도 설정

        int code = FindAnyObjectByType<LocalPlayerSetter>().teamCode;
        CMDSetCharacterInfo(info.hp, info.attack, info.speed, info.attackableRange, code);
    }

    public void SetCamera()
    {
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;
    }

    [Command]
    public void CMDSetCharacterInfo(int hp, int attack, float speed, float attackableRange, int teamCode)
    {
        this.fullHp = hp;
        this.hp = hp;
        this.attack = attack;

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
        OnChangeCurrentHpBar((float)newVal / fullHp);
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


    #region     
    [Server]
    public void ApplyDamage(int amount)
    {
        Debug.Log($"{amount} -> Damage Apply");
        hp -= amount;
        hp = Mathf.Clamp(hp, 0, fullHp);
    }

    [Server]
    public void ApplyHeal(float amount)
    {
        Debug.Log("Heal Apply");
        hp += (int)amount;
        hp = Mathf.Clamp(0, fullHp, hp);
    }

    [Server]
    public void ApplySlow(float duration, float amount)
    {
        Debug.Log("Slow Apply");
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => speed *= (amount / 100f))
            .AppendInterval(duration)
            .OnComplete(() => speed = data.speed);
    }

    [Server]
    public void ApplyDot(float duration, float amount)
    {
        Debug.Log("Dot Apply");
        int tickCount = Mathf.FloorToInt(duration);
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < tickCount; i++)
        {
            seq.AppendInterval(1f) // 1초 대기
            .AppendCallback(() => ApplyDamage((int)amount)); // 데미지 적용
        }
    }
    #endregion
}