using UnityEngine;

public class EffectHandler
{
    PlayerController player;
    public EffectHandler(PlayerController player)
    {
        this.player = player;
    }

    public void TakeDamage(int amount)
    {
        int testMax = 100;
        player.currentHp -= amount;
        player.currentHp = Mathf.Clamp(0, testMax, player.data.hp);
    }

    public void ApplySlow(float duration, float amout)
    {
        // 스피드 줄이기.
    }
}