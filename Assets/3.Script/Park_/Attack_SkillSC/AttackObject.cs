using Mirror;
using UnityEngine;

public class AttackObject : NetworkBehaviour
{
    PlayerController caster;
    private int damage;
    public void SetCaster(PlayerController caster, int damage)
    {
        this.caster = caster;
        this.damage = damage;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Attack Trigger!");

        //적인 경우.
        if (other.gameObject.TryGetComponent(out PlayerController player) && player.teamCode != caster.teamCode)
        {
            Debug.Log("enemy 피격!");
            player.effectHandler.ApplyDamage(damage);
            Destroy(this.gameObject);
        }

        // 상대편 타워 인경우
        if (other.gameObject.TryGetComponent(out TowerControl tower) && tower.teamCode != caster.teamCode)
        {
            Debug.Log("Tower 피격!");
            tower.ApplyDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
