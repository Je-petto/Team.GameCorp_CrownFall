using UnityEngine;

public class AttackObject : MonoBehaviour
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
        //적인 경우.
        if (other.GetComponent<PlayerController>().teamCode != caster.teamCode)
        {
            other.GetComponent<PlayerController>().effectHandler.ApplyDamage(damage);
            Destroy(this.gameObject);
        }

        // 상대편 타워 인경우
        if (other.GetComponent<TowerControl>().teamCode != caster.teamCode)
        {
            other.GetComponent<TowerControl>().ApplyDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
