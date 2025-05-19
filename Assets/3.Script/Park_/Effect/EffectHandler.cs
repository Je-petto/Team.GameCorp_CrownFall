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
        player.hp -= amount;
        player.hp = Mathf.Clamp(0, testMax, player.hp);            
    }
}