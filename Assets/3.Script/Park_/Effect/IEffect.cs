using UnityEngine;

public interface IEffect
{
    public abstract void Apply(PlayerController player);
}

public class DamageEffect : IEffect
{
    private int amount;
    public DamageEffect(int amount){
        this.amount = amount;
    }
    public void Apply(PlayerController player)
    {
        player.effectHandler.TakeDamage(amount);
    }
}

public class SlowEffect : IEffect
{
    public SlowEffect()
    {

    }
    public void Apply(PlayerController player)
    {
    }
}