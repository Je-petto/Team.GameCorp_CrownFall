using System.Collections;
using Mirror;
using UnityEngine;

public class AttackObject : NetworkBehaviour
{
    PlayerController caster;
    private int damage;

    private float moveDuration = 0.8f;
    Vector3 targetPoint;
    public void SetCaster(PlayerController caster, int damage, Vector3 point)
    {
        this.caster = caster;
        this.damage = damage;
        this.moveDuration = 0.8f;

        targetPoint = point;
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

    #region Network
    public override void OnStartServer()
    {
        base.OnStartServer();
        StartCoroutine(MoveAndDestroy());
    }

    [Server]
    private IEnumerator MoveAndDestroy()
    {
        float time = 0f;
        Vector3 start = transform.position;

        // 0.8은 지속시간
        while (time < moveDuration)
        {
            transform.position = Vector3.Lerp(start, caster.targetPoint, time / moveDuration);
            time += Time.deltaTime;
            yield return null;
        }

        transform.position = caster.targetPoint;
        NetworkServer.Destroy(gameObject); // 서버에서 제거 => 클라이언트도 제거됨
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        // 서버에서만 충돌 처리
        if (other.TryGetComponent(out PlayerController player) && player.teamCode != caster.teamCode)
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
    
    #endregion
}
