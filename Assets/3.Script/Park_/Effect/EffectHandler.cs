using UnityEngine;

public class EffectHandler : MonoBehaviour
{
    PlayerController player;
    public EffectHandler(PlayerController player)
    {
        this.player = player;
    }

    public void TakeDamage(int amount)
    {
        player.currentHp -= amount;
        player.currentHp = Mathf.Clamp(0, player.data.hp, player.currentHp);
    }

    public void ApplySlow(float duration, float amount)
    {

    }


    public void ApplyDot(float duration, float amount)
    {

    }
}