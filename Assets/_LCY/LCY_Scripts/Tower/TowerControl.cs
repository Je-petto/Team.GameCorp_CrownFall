using CustomInspector;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Mirror;
using System;

[Serializable]
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

    public int teamCode_Inspector;

    [SyncVar(hook = nameof(OnInitTeam))]
    public int teamCode = -1;                    //인스펙터에서 설정하기

    [CustomInspector.ReadOnly] public Collider col;

    [HorizontalLine("TOWER STATE"), HideField] public bool b1;
    [CustomInspector.ReadOnly] public bool protect = false;
    [CustomInspector.ReadOnly] public bool recovery = false;
    [CustomInspector.ReadOnly] public bool isHit = false;
    [CustomInspector.ReadOnly] public bool isDestroy = false;

    [HorizontalLine("SHIELD"), HideField] public bool b2;
    [CustomInspector.ReadOnly] public GameObject shield;
    [CustomInspector.ReadOnly] public ParticleSystem shieldParticle;
    [SyncVar] public int shieldHp;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("HealthHPBar")]
    [SerializeField] private Image teamColorHpbar;

    [CustomInspector.ReadOnly] public float maxHp;

    [SyncVar(hook = nameof(OnHpChanged))]
    private int hp;

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
        hp = state.health;
        maxHp = state.health;

        Debug.Log("타워 생성!");
    }

    private void Update()
    {
        SetShieldPosition();
        
        if (state.health <= 0) DestroyTower();
    }

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


    private void SetShieldPosition()
    {
        shield.transform.position = transform.position;
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

    [Server]
    public void ApplyDamage(int damage)
    {
        if (!isServer) return;

        hp -= damage;
        hp = Mathf.Clamp(hp, 0, (int)maxHp);


        if (!protect && hp <= (maxHp * 0.6f))
        {
            //실드 실시.
            StartProtect();
        }
    }

    void StartProtect()
    {
        shield.SetActive(true);
        shieldParticle.Play();
    }

    void OnHpChanged(int oldVal, int newVal)
    {
        UpdateHpBar();
    }

    void UpdateHpBar()
    {
        teamColorHpbar.fillAmount = (float)hp / maxHp;
    }    
}