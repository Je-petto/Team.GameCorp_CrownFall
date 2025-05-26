using CustomInspector;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mirror;

public struct TowerState
{
    public int health;
    public int shieldHealth;

    public void Set(TowerProfile profile)
    {
        health = profile.health;
        shieldHealth = profile.shieldHealth;
    }
}

public class TowerControl : NetworkBehaviour
{
    public TowerProfile profile { get => towerProfile; set => towerProfile = value; }
    [SerializeField] private TowerProfile towerProfile;
    public TowerState state;
    public int teamCode;                    //인스펙터에서 설정하기

    [CustomInspector.ReadOnly] public float maxHealth;
    [CustomInspector.ReadOnly] public Collider col;

    [HorizontalLine("DEBUG"), HideField] public bool b0;
    [SerializeField] private int debugHealth;
    [SerializeField] private int debugShieldHealth;

    [HorizontalLine("TOWER STATE"), HideField] public bool b1;
    [CustomInspector.ReadOnly] public bool protect = false;
    [CustomInspector.ReadOnly] public bool recovery = false;
    [CustomInspector.ReadOnly] public bool isHit = false;
    [CustomInspector.ReadOnly] public bool isDestroy = false;

    [HorizontalLine("SHIELD"), HideField] public bool b2;
    [CustomInspector.ReadOnly] public GameObject shield;
    [CustomInspector.ReadOnly] public ParticleSystem shieldParticle;

    [HorizontalLine("???"), HideField] public bool b3;
    [SerializeField, Tooltip("회복량")] private int heelAmount;
    [SerializeField, Tooltip("회복 간격")] private int heelDelay;
    [SerializeField, Tooltip("회복 시작까지 대기 시간")] private int recoveryDelay;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("HealthHPBar")]
    [SerializeField] private Image teamColorHpbar;

    [SyncVar(hook = nameof(OnSyncHealthChanged))]
    private int syncedHealth;
    
    private float rDelay;
    private float hDelay;
    private bool isGameOver = false;

    private void Awake()
    {
        shield = Instantiate(towerProfile.shieldModel);
        shield.SetActive(false);

        col = GetComponent<Collider>();
    }

    private void Start()
    {
        teamColorHpbar.color = teamCode == 0 ? Color.red : Color.blue;

        state.Set(towerProfile);
        syncedHealth = state.health;
        maxHealth = state.health;

        Debug.Log("타워 생성!");
    }

    private void Update()
    {
        debugHealth = state.health;
        debugShieldHealth = state.shieldHealth;

        SetShieldPosition();
        OnProtect();
        OnRecovery();

        if(state.health <= 0) DestroyTower();
    }

    private void SetShieldPosition()
    {
        shield.transform.position = transform.position;
    }

    private void OnProtect()
    {
        if (state.health <= (maxHealth * 0.6f))
            protect = true;

        if (protect)
        {
            shield.SetActive(true);
            //shieldParticle.Play();
        }

        if (state.shieldHealth <= 0)
        {
            protect = false;
            shield.SetActive(false);
        }
    }

    private void OnRecovery()
    {
        if (state.health <= (maxHealth * 0.25f) && !isHit)
            recovery = true;

        if (recovery)
        {
            if (isHit)
            {
                recovery = false;
                return;
            }
            rDelay += Time.deltaTime;
            hDelay += Time.deltaTime;
            if (rDelay >= recoveryDelay && hDelay >= heelDelay)
            {
                hDelay = 0;
                state.health += heelAmount;
                OnChangeHpbar();
                if (state.health == (maxHealth / 2))
                {
                    rDelay = 0;
                    recovery = false;
                    return;
                }
            }
        }
    }

    private void DestroyTower()
    {
        isDestroy = true;
        isHit = false;
        gameObject.SetActive(false);
        col.isTrigger = false;

        Time.timeScale = 0f;

        GameManager.I?.OnGameWin();

        int winnerTeam = (this.teamCode == 0) ? 1 : 0;
        StartCoroutine(ShowGameOverPanel_Co(winnerTeam));
    }

    private IEnumerator ShowGameOverPanel_Co(int teamcode)
    {
        yield return new WaitForSecondsRealtime(2f);

        string winnerTeam = teamcode == 0 ? "RED" : "BLUE";
        UIManager.I?.ShowGameEndPanel(winnerTeam);
    }

    public void ApplyDamage(int damage)
    {
        if (!isServer) return;

        syncedHealth -= damage;
        syncedHealth = Mathf.Clamp(syncedHealth, 0, (int)maxHealth);
    }
    private void OnSyncHealthChanged(int oldValue, int newValue)
    {
        state.health = newValue;
        OnChangeHpbar();
    }
    
    private void OnChangeHpbar()
    {
        teamColorHpbar.fillAmount = syncedHealth / maxHealth;
    }
}