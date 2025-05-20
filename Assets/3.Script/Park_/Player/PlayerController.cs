using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using Unity.VisualScripting;
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
public class PlayerController : MonoBehaviour
{
    #region PlayerStat
    public int currentHp;
    public CharacterInfo data;
    public List<EffectType> effectTypes = new();
    #endregion

    //[HideInspector]
    public Rigidbody rb;
    public Animator animator;

    public PlayerStateMachine stateMachine;
    public PlayerInputHandler inputHandler;
    public EffectHandler effectHandler;

    #region Test
    public TeamComponent teamData = null;
    public PlayerController target = null;
    #endregion

    void Start()
    {
        //test Code
        teamData = new(TeamType.Red);
    }

    public void SetCharacter(CharacterInfo data)
    {
        this.data = data;
        currentHp = data.hp;
        Instantiate(data.model, transform.Find("_mesh"));

        // 로컬 테스트용.
        StartCoroutine(InitComponents_Co());
    }

    IEnumerator InitComponents_Co()
    {
        yield return new WaitForSeconds(0.5f);

        TryGetComponent(out rb);
        TryGetComponent(out stateMachine);
        TryGetComponent(out inputHandler);

        animator = GetComponentInChildren<Animator>();
        animator.runtimeAnimatorController = data.inGameAnimator;

        // 카메라 => Cinemachine 세팅
        FindObjectOfType<CinemachineVirtualCamera>().Follow = transform;

        // 동적 생성
        effectHandler = new EffectHandler(this);

        yield return new WaitUntil(() => inputHandler != null);
        
        inputHandler.moveCommand = new MoveCommand(this);
        inputHandler.attackCommand = new AttackCommand(this, new PlayerAttack(this));
        inputHandler.detectCommand = new DetectionCommand(this, new PlayerDetection(this));
    }

    void OnDrawGizmos()
    {
        if (data == null) return;

        //공격 범위 드로우
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, data.attackableRange);
    }
}