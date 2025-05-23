using System.Collections;
using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IPlayerState
{
    public void Enter() { }

    public void Update(){ }

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
        player.inputHandler.enabled = false;

        Debug.Log("죽음");
        player.StartCoroutine(Respawn_Co());
        IngameUIManager.Instance.ShowRespawnUI(respawnTime);
    }

    public void Update() {}

    public void Exit()
    {
        player.inputHandler.enabled = true;
        IngameUIManager.Instance.HideRespawnUI();
    }

    private IEnumerator Respawn_Co()
    {
        float timer = respawnTime;
        while (timer > 0)
        {
            IngameUIManager.Instance.UpdateRespawnTime(timer);
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        player.transform.position = SpawnManager.Instance.GetSpawnPoint(player.teamData.type);
        player.currentHp = player.data.hp;
        player.stateMachine.ChangeState(new IdleState());
    }
}