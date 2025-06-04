using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mirror;

public class TowerControl : NetworkBehaviour
{
    public TowerProfile profile { get => towerProfile; set => towerProfile = value; }
    [SerializeField] private TowerProfile towerProfile;

    public int teamCode_Inspector;

    [SyncVar(hook = nameof(OnInitTeam))]
    public int teamCode = -1;                    //인스펙터에서 설정하기

    [CustomInspector.ReadOnly] public Collider col;

    [Header("HealthHPBar")]
    [SerializeField] private Image teamColorHpbar;

    [CustomInspector.ReadOnly] public float maxHp;

    [SyncVar(hook = nameof(OnHpChanged))]
    public int hp;

    private void Awake()
    {
        TryGetComponent(out col);
    }

    private void Start()
    {
        Debug.Log("타워 생성!");
        hp = towerProfile.health;
        maxHp = towerProfile.health;
        sHp = towerProfile.shieldHealth;
        sMaxHp = towerProfile.shieldHealth;

        this.teamCode = teamCode_Inspector;
        shieldbar.enabled = false;
        shield.SetActive(false);
        isProtecting = false;
    }

    private void Update()
    {
        if (hp <= 0) DestroyTower();
    }

    void OnInitTeam(int oldVal, int newVal)
    {
        Debug.Log("TeamCode Change!!");
        teamColorHpbar.color = newVal == 0 ? Color.red : Color.blue;
        if (teamColorHpbar == null)
        {
            Debug.Log("teamColorHpbar is null...");
            return;
        }
    }

    private void DestroyTower()
    {
        Debug.Log("Destroy Tower...");
        col.isTrigger = false;

        string winnerTeam = (this.teamCode == 0) ? "RED" : "BLUE";
        (NetworkManager.singleton as InGameNetworkManager).GameOver(this.gameObject, winnerTeam);

        StartCoroutine(ShowGameOverPanel_Co());
    }

    [SerializeField] ParticleSystem destoryParticle;

    [ClientRpc]
    private void RpcStartParticle()
    {
        if (destoryParticle != null)
        {
            destoryParticle.Play();
        }
        else
        {
            Debug.LogWarning("Destroy particle is not assigned!");
        }
    }

    private IEnumerator ShowGameOverPanel_Co()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        RpcStartParticle();
        this.gameObject.SetActive(false);
    }

    [Server]
    public void ApplyDamage(int damage)
    {
        if (!isServer)
        {
            Debug.Log("No Server...");
            return;
        }

        if (isProtecting)
        {
            sHp -= damage;
            if (sHp <= 0)
            {
                BreakShield();
            }
            return;            
        }

        hp -= damage;
        hp = Mathf.Clamp(hp, 0, (int)maxHp);

        if (this.shield == null) return;

        if (!isProtecting && hp <= (maxHp * 0.6f))
        {
            //실드 생성.
            this.isProtecting = true;
            StartProtect();
        }
    }

    void OnActiveShield(bool oldVal, bool newVal)
    {
        if (oldVal == true && newVal == false)
        {
            Debug.Log($"Shield Remove");
            Destroy(this.shield);
            return;
        }
        
        shield.SetActive(newVal);
        shieldbar.enabled = newVal;
    }
    
    void StartProtect()
    {
        SpawnShield();
    }

    [Command]
    void SpawnShield()
    {
        this.isProtecting = true;
    }

    //hook
    void OnHpChanged(int oldVal, int newVal)
    {
        UpdateHpBar(newVal);
    }

    void UpdateHpBar(int currentHp)
    {
        teamColorHpbar.fillAmount = (float)currentHp / maxHp;
    }

    #region Shield
    [SyncVar(hook = nameof(OnActiveShield))]
    public bool isProtecting = false;
    
    [Header("ShieldPrefab")]
    [SerializeField] GameObject shield;
    [SerializeField] private Image shieldbar;

    [SyncVar(hook = nameof(OnShieldHpChanged))]
    public int sHp;

    [SyncVar]
    public int sMaxHp;

    void OnShieldHpChanged(int oldVal, int newVal)
    {
        UpdateShieldBar(newVal);
    }

    void UpdateShieldBar(int currentHp)
    {
        shieldbar.fillAmount = (float)currentHp / maxHp;
    }

    void BreakShield()
    {
        Debug.Log("Break Shield!!!");
        this.isProtecting = false;
        shieldbar.enabled = false;
        Destroy(this.shield.gameObject);
    }
    #endregion
}