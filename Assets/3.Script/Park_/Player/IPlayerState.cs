using UnityEngine;

public interface IPlayerState
{
    void Enter();
    void Update();
    void Exit();
}

public class IdleState : IPlayerState
{
    public void Enter()
    {
        Debug.Log("Idle Enter...");
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        Debug.Log("Idle Exit...");
    }
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

    public void Enter()
    {
        // Debug.Log("Move Enter...");
    }

    public void Update()
    {
        Move();
        Rotate();
    }

    public void Exit()
    {
        // Debug.Log("Move Exit...");
    }

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