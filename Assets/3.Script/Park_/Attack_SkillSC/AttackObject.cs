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
    public void SetAttack(PlayerController_Net caster, int damage, Vector3 targetPoint)
    {
        Debug.Log($"target Point {targetPoint}");
        this.caster = caster;
        this.damage = damage;
           
        Shoot(targetPoint);
    }

    [Server]
    void Shoot(Vector3 point)
    {
        Debug.Log("Attack OBject Shoot!!!");
        transform.DOMove(point, moveDuration)
            .OnComplete(() =>
            {
                if (this != null && gameObject != null)
                    NetworkServer.Destroy(gameObject);
            });
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        
        if (caster == null)
        {
            Debug.LogWarning("Caster is null!");
            return;
        }


        // 서버에서만 충돌 처리
        if (other.TryGetComponent(out PlayerController_Net player) && player.teamCode != caster.teamCode)
        {
            Debug.Log($"Enemy Trigger!!!");
            player.ApplyDamage(damage);
            NetworkServer.Destroy(this.gameObject);
        }

        if (other.TryGetComponent(out TowerControl tower) && tower.teamCode != caster.teamCode)
        {
            Debug.Log($"Tower Trigger!!!");
            tower.ApplyDamage(1000);
            NetworkServer.Destroy(this.gameObject);
        }
    }
}