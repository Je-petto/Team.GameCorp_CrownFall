using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Mirror;
using TMPro;
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
    #endregion

    public Vector3 targetPoint;

    [Header("Player Spawn")]
    public Vector3 spawnPoint = Vector3.zero;

    [Header("GUI")]
    [SerializeField] GameObject playerUI;               //parent
    [SerializeField] GameObject gameOverPanl;
    [SerializeField] TextMeshProUGUI winnerTeam;
    [SerializeField] Image faceImage;
    [SerializeField] Image skillImage;
    [SerializeField] Image elementIcon;
    [SerializeField] public Image skillCoolDownImage;

    [Header("Damage Particle")]
    public GameObject[] effectParticles;


    public override void OnStartClient()
    {
        Debug.Log("[Client] client Start!");
        base.OnStartClient();

        StartCoroutine(InitComponents());

        if (isLocalPlayer)
        {
            // data = FindAnyObjectByType<LocalPlayerSetter>().info;
            StartCoroutine(InitPlayerDatas());
        }
        else
        {
            playerUI.SetActive(false);
        }
    }

    private IEnumerator InitPlayerDatas()
    {
        yield return new WaitUntil(() => InGameSession.isInit);

        Debug.Log("PlayerDatas INIT....");
        string myUid = InGameSession.uid;

        // 서버에 요청
        CMDRequestCharacterId(myUid);
    }

    [Command]
    public void CMDRequestCharacterId(string uid)
    {
        string cid = ((InGameNetworkManager)NetworkManager.singleton).userList
                        .Find(u => u.uid == uid)?.c_id;

        int tc = ((InGameNetworkManager)NetworkManager.singleton).userList
                        .Find(u => u.uid == uid).teamCode;

        if (string.IsNullOrEmpty(cid))
        {
            Debug.LogError("[Server] 해당 uid에 대한 cid를 찾을 수 없음");
            return;
        }
        Debug.Log($"[Server] Request : cid = {cid}");
        TargetReceiveCharacterData(connectionToClient, cid, tc);
    }

    [TargetRpc]
    public void TargetReceiveCharacterData(NetworkConnection target, string cid, int tc)
    {
        Debug.Log($"[Client] 서버로부터 받은 CID: {cid}");
        Debug.Log($"[Client] 서버로부터 받은 팀코드 : {tc}");

        InGameSession.characterId = cid;

        this.data = ((InGameNetworkManager)NetworkManager.singleton)
                        .characterInfos.Find(c => c.cid == cid);

        if (data == null)
        {
            Debug.LogError("캐릭터 데이터 초기화 실패");
            return;
        }

        StartCoroutine(InitComponents());
        CMDSetCID(cid, tc);
        SetCamera();
        StartCoroutine(SetLocalUI());
    }


    [Command]
    public void CMDSetCID(string cid, int teamCode)
    {
        this.cid = cid;
        this.teamCode = teamCode;
    }

    void OnCidChanged(string oldCid, string newCid)
    {
        Debug.Log($"[Client] cid changed → {newCid}");

        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == newCid);
        if (info == null || info.inGameModel == null)
        {
            Debug.Log($"info is null...");
            return;
        } else {
            Debug.Log($"info is {info}...");
            Debug.Log($"info is {info.inGameModel.name}...");
        }
        data = info;

        Transform mesh = transform.Find("_mesh");
        if (mesh.childCount > 0) return;

        GameObject model = Instantiate(info.inGameModel, mesh);
        CMDSetCharacterInfo(info.hp, info.attack, info.speed, info.attackableRange);
    }

    public void SetCamera()
    {
        var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (cam != null) cam.Follow = transform;
    }

    [Command]
    public void CMDSetCharacterInfo(int hp, int attack, float speed, float attackableRange)
    {
        Debug.Log("CMD CharacterInfo!");
        this.fullHp = hp;
        this.hp = hp;
        this.attack = attack;

        this.speed = speed;
        this.attackableRange = attackableRange;
    }

    IEnumerator SetLocalUI()
    {
        if (!isLocalPlayer) yield break;

        yield return new WaitUntil(() => data != null);

        Debug.Log("Local ui Init!");

        this.faceImage.sprite = data.face;
        this.skillImage.sprite = data.SkillIcon;
        this.elementIcon.sprite = data.ElementIcon;
    }

    IEnumerator InitComponents()
    {
        Debug.Log($"[Cllent] InitComponents...");

        yield return new WaitUntil(() => cid != null && cid != "" && data != null);

        Debug.Log($"[Client] Start Components");
        attackPoint = transform.Find("_attackPoint");

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);
        TryGetComponent(out lineRenderer);

        yield return new WaitUntil(() => transform.Find("_mesh").childCount > 0);

        animator = GetComponentInChildren<Animator>();

        if (lineRenderer != null) lineRenderer.enabled = false;

        yield return new WaitUntil(() => data != null && inputHandler != null);

        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.detectCommand = new DetectCommand(this, new(this));
        inputHandler.attackCommand = new AttackCommand(this, new(this));

        ISkillAction skill = SkillFactory.CreateSkillAction(this, data.skillSet);

        if (skill != null)
        {
            Debug.Log("skill Set Complete!..");
            inputHandler.skillCastCommand = new SkillCastCommand(this, this.data.skillSet, skill);
        }
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

        Sequence seq = DOTween.Sequence();

        seq.AppendInterval(0.4f)
            .AppendCallback(() =>
            {
                GameObject attackOrb = Instantiate(projection, transform.position, Quaternion.identity);
                Debug.Log("network spawn ready...");
                NetworkServer.Spawn(attackOrb);

                Debug.Log($" Set Target : {targetPoint}");
                attackOrb.GetComponent<AttackObject>().SetAttack(this, info.attack, targetPoint);
            });
    }

    [Command]
    public void CMDCastSkill(Vector3 targetPoint)
    {
        CharacterInfo info = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid);
        SkillData skill = ((InGameNetworkManager)NetworkManager.singleton).characterInfos.Find(c => c.cid == cid).skillSet;

        if (skill == null)
        {
            Debug.Log("Skill data is null...");
        }

        GameObject skillObject = null;

        List<IEffect> effects = EffectFactory.CreateSkillEffects(skill);

        Sequence skillSeq = DOTween.Sequence();
        skillSeq.AppendInterval(skill.castingTime)
                .AppendCallback(() =>
                {
                    skillObject = Instantiate(skill.prefab, targetPoint, Quaternion.identity);
                    NetworkServer.Spawn(skillObject);
                    skillObject.GetComponent<SkillEffectController>().SetProps(this, effects);
                })
                .AppendInterval(skill.duration)
                .AppendCallback(() =>
                {
                    NetworkServer.Destroy(skillObject);
                    Destroy(skillObject);
                });
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

        if (newVal == 0)                  // Red
        {
            spawnPoint = new Vector3(15f, 0, 35f);
            transform.position = new Vector3(15f, 0, 35f);
        }
        else if (newVal == 1)             // Blue
        {
            spawnPoint = new Vector3(15f, 0, 8f);
            transform.position = new Vector3(15f, 0, 8f);
        }
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

        if (hp <= 0 && !inputHandler.isDeath)
        {
            RPCDie();
        }
    }

    [ClientRpc]
    public void RPCDie()
    {
        Debug.Log("Die RPC Received");
        inputHandler.isDeath = true;
        inputHandler.deathCommand = new DeathCommand(this, new DeadState(this));
        inputHandler.deathCommand.Execute();
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


    [Command]
    public void CMDRespawn()
    {
        hp = fullHp;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
    }

    [Command]
    public void CMDPlayAnimationTrigger(string t)
    {
        RpcPlayAnimationTrigger(t);
    }

    [Command]
    public void CMDPlayAnimationFloat(float f)
    {
        RpcPlayerMoveAnimation(f);
    }

    [ClientRpc]
    public void RpcPlayAnimationTrigger(string t)
    {
        if (animator == null)
        {
            Debug.Log("[Trigger] animator is null..");
            return;
        }

        animator.SetTrigger(t);
    }

    [ClientRpc]
    public void RpcPlayerMoveAnimation(float f)
    {
        if (animator == null)
        {
            Debug.Log("[float] animator is null..");
            return;
        }
        animator.SetFloat("Movement", f);
    }

    [TargetRpc]
    public void GameOverSet(GameObject target, string team)
    {
        Debug.Log("GameOver...");
        inputHandler.isStop = true;
        Sequence s = DOTween.Sequence();
        s.AppendCallback(() =>
        {
            var cam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
            if (cam != null) cam.Follow = target.transform;
        })
        .AppendInterval(2f)
        .AppendCallback(() =>
        {
            gameOverPanl.SetActive(true);
            winnerTeam.text = $"{team} Team Wins";
        });
    }
}