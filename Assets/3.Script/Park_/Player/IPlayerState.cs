using System.Collections;
using DG.Tweening;
using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IPlayerState
{
    PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }
    public void Enter()
    {
        player.animator.SetFloat("Movement", 0f);
    }
    public void Update(){ 
        
    }
    public void Exit() { }
}

public class MoveState : IPlayerState
{
    private PlayerController player;

    private Vector3 direction;

    public MoveState(PlayerController player)
    {
        //PlayerController에 각종 스탯을 가지고 있다.
        this.player = player;
    }

    public void Enter() { }

    public void Update()
    {
        Move();
        Rotate();
    }

    public void Exit() { }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        direction = new Vector3(h, 0, v).normalized;

        Vector3 vel = direction * player.data.speed;
        player.rb.MovePosition(player.transform.position + vel * Time.deltaTime);
        player.animator.SetFloat("Movement", direction.magnitude);
    }

    void Rotate()
    {
        if (direction == Vector3.zero) return;

        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(player.transform.eulerAngles.y, angle, ref player.data.rotateSpeed, 0.1f);
        player.transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);
    }
}


public class DeadState : IPlayerState
{
    private PlayerController player;
    private float respawnTime;

    public DeadState(PlayerController player)
    {
        this.player = player;
        this.respawnTime = player.respawnTime; // ⬅ 플레이어에서 설정된 시간 사용
    }

    public void Enter()
    {
        player.animator.SetTrigger("Death");
        player.pState = LifeState.DEATH;
        player.inputHandler.enabled = false;

        StartDeathSequence();
    }

    public void Update() {}

    public void Exit()
    {
        player.inputHandler.enabled = true;
    }

    private void StartDeathSequence()
    {
        Sequence seq = DOTween.Sequence();

        int elapsed = (int)respawnTime;

        seq.AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Debug.Log("죽음...");
                IngameUIManager.I.ShowRespawnUI(respawnTime);
            });

        for (int i = elapsed; i > 0; i--)
        {
            int time = i; // 클로저 문제 방지
            seq.AppendCallback(() =>
            {
                IngameUIManager.I.UpdateRespawnTime(time);
                Debug.Log($"Respawn remain: {time}");
            });
            seq.AppendInterval(1f);
        }

        seq.OnComplete(() =>
        {
            Debug.Log("리스폰...");
            IngameUIManager.I.HideRespawnUI();

            // 위치 초기화
            player.transform.position = Vector3.zero;
            player.transform.rotation = Quaternion.identity;

            // 체력 회복
            player.currentHp = player.data.hp;

            // UI 숨기기

            // 애니메이션 초기화
            player.animator.SetFloat("Movement", 0f);

            // 상태 전환
            player.stateMachine.ChangeState(new IdleState(player));
        });
    }
}