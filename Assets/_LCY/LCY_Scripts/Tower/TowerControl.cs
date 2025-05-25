using CustomInspector;
using UnityEngine;
using static UnityEngine.ParticleSystem;
using System.Collections;

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

public class TowerControl : MonoBehaviour
{
    public TowerProfile profile { get => towerProfile; set => towerProfile = value; }
    [SerializeField] private TowerProfile towerProfile;
    public TowerState state;
    public int teamCode;                    //인스펙터에서 설정하기

    [ReadOnly] public float maxHealth;
    [ReadOnly] public Collider col;

    [HorizontalLine("DEBUG"), HideField] public bool b0;
    [SerializeField] private int debugHealth;
    [SerializeField] private int debugShieldHealth;

    [HorizontalLine("TOWER STATE"), HideField] public bool b1;
    [ReadOnly] public bool protect = false;
    [ReadOnly] public bool recovery = false;
    [ReadOnly] public bool isHit = false;
    [ReadOnly] public bool isDestroy = false;

    [HorizontalLine("SHIELD"), HideField] public bool b2;
    [ReadOnly] public GameObject shield;
    [ReadOnly] public ParticleSystem shieldParticle;

    [HorizontalLine("???"), HideField] public bool b3;
    [SerializeField, Tooltip("회복량")] private int heelAmount;
    [SerializeField, Tooltip("회복 간격")] private int heelDelay;
    [SerializeField, Tooltip("회복 시작까지 대기 시간")] private int recoveryDelay;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    private float rDelay;
    private float hDelay;
    private bool isGameOver = false;

    private void Awake()
    {
        shield = Instantiate(towerProfile.shieldModel);
        shield.SetActive(false);


        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        maxHealth = state.health;
    }

    private void Update()
    {
        debugHealth = state.health;
        debugShieldHealth = state.shieldHealth;

        SetShieldPosition();
        OnProtect();
        OnRecovery();
        DestroyTower();
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

    private IEnumerator ShowGameOverPanel_Co(int winnerTeam)
    {
        yield return new WaitForSecondsRealtime(2f);

        UIManager.I?.ShowGameEndPanel(winnerTeam.ToString());
    }

    public void ApplyDamage(int damage)
    {
        //hp 데미지 주기
    }
}