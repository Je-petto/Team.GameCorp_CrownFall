using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public enum TeamType
{
    Red,
    Blue
}
public class TeamComponent
{
    public TeamType type;

    public TeamComponent(TeamType type)
    {
        this.type = type;
    }

    public bool IsEnemy(TeamComponent component)
    {
        return type != component.type;
    }
}

public class PlayerController : NetworkBehaviour
{
    // public class PlayerStat
    // {
    //     public float moveSpeed;
    //     public float rotateSpeed;
    //     public int attackDamage;        
    // }

    #region PlayerStat
    public int hp;                                  // 체력
    public float moveSpeed;                         // 이동 속도
    public float rotateSpeed;                       // 회전 속도
    public int attackDamage;                        // 공격력
    public float attackableRange;                   // 공격 가능 범위
    public List<EffectType> effectTypes = new();
    #endregion

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Animator animator;

    public PlayerStateMachine stateMachine;
    private PlayerInputHandler inputHandler;
    public EffectHandler effectHandler;

    #region Test
    public TeamComponent teamData = null;
    public PlayerController target = null;
    #endregion

    void Start()
    {
        if (!isLocalPlayer) return;

        InitComponents();

        //test Code
        teamData = new(TeamType.Red);
    }

    void InitComponents()
    {
        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);

        animator = GetComponentInChildren<Animator>();

        // 카메라 => Cinemachine 세팅
        FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;

        // 동적 생성
        effectHandler = new EffectHandler(this);

        // 수정 예정
        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.attackCommand = new AttackCommand(this, new PlayerAttack(this));
        inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));
    }

    void OnDrawGizmos()
    {
        //공격 범위 드로우
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, attackableRange);
    }
}