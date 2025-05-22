using UnityEngine;

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
    // Debug
    [SerializeField] private int debugHealth;
    [SerializeField] private int debugShieldHealth;
    // Debug

    public float maxhp;

    public TowerProfile profile { get => towerProfile; set => towerProfile = value; }
    [SerializeField] private TowerProfile towerProfile;
    public TowerState state;

    public Collider col;

    public bool protect = false;
    public bool recovery = false;
    public bool isHit = false;
    public bool isDestroy = false;

    public GameObject shield;

    private float timer;
    private float delay;

    private void Awake()
    {
        shield = Instantiate(towerProfile.shieldModel);
        shield.SetActive(false);

        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        maxhp = state.health;
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
        if (state.health <= 60)
            protect = true;

        if (protect)
            shield.SetActive(true);

        if (state.shieldHealth <= 0)
        {
            protect = false;
            shield.SetActive(false);
        }
    }


    private void OnRecovery()
    {
        if (state.health <= 25 && !isHit)
            recovery = true;

        if (recovery)
        {
            if (isHit)
            {
                recovery = false;
                return;
            }
            timer += Time.deltaTime;
            delay += Time.deltaTime;
            if (timer >= 3 && delay >= 1)
            {
                delay = 0;
                state.health += 10;
                if (state.health == maxhp / 2)
                {
                    timer = 0;
                    recovery = false;
                    return;
                }
            }
        }
    }

    private void DestroyTower()
    {
        if (state.health <= 0)
            isDestroy = true;

        if (isDestroy)
        {
            isHit = false;
            gameObject.SetActive(false);
            col.isTrigger = false;
        }
    }
}