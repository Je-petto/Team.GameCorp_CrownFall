using DG.Tweening;
using Mirror;
using UnityEngine;

public class AttackObject : NetworkBehaviour
{
    [SyncVar]
    public PlayerController_Net caster;

    [SyncVar]
    public int damage;

    public Vector3 point;

    private float moveDuration = 0.8f;

    [Server]
    public void SetAttack(PlayerController_Net caster, int damage, Vector3 target)
    {
        this.caster = caster;
        this.damage = damage;
        this.point = target;
           
        Shoot(target);
    }

    void Shoot(Vector3 point)
    {
        Debug.Log("Attack OBject Shoot!!!");
        transform.DOMove(point, moveDuration).OnComplete(() =>
        {
            NetworkServer.Destroy(gameObject);
        });
    }

    // void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Attack Trigger!");

    //     //적인 경우.
    //     if (other.gameObject.TryGetComponent(out PlayerController player) && player.teamCode != caster.teamCode)
    //     {
    //         Debug.Log("enemy 피격!");
    //         player.effectHandler.ApplyDamage(damage);
    //         Destroy(this.gameObject);
    //     }

    //     // 상대편 타워 인경우
    //     if (other.gameObject.TryGetComponent(out TowerControl tower) && tower.teamCode != caster.teamCode)
    //     {
    //         Debug.Log("Tower 피격!");
    //         tower.ApplyDamage(damage);
    //         Destroy(this.gameObject);
    //     }
    // }

    // [Server]
    // private IEnumerator MoveAndDestroy()
    // {
    //     Debug.Log("Move Destroy...");
    //     float time = 0f;
    //     Vector3 start = transform.position;

    //     // 0.8은 지속시간
    //     while (time < moveDuration)
    //     {
    //         transform.position = Vector3.Lerp(start, caster.targetPoint, time / moveDuration);
    //         time += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.position = caster.targetPoint;
    //     NetworkServer.Destroy(gameObject); // 서버에서 제거 => 클라이언트도 제거됨
    // }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        // 서버에서만 충돌 처리
        if (other.TryGetComponent(out PlayerController_Net player) && player.teamCode != caster.teamCode)
        {
            player.effectHandler.ApplyDamage(damage);
            NetworkServer.Destroy(gameObject);
        }

        if (other.TryGetComponent(out TowerControl tower) && tower.teamCode != caster.teamCode)
        {
            tower.ApplyDamage(damage);
            NetworkServer.Destroy(gameObject);
        }
    }
}
