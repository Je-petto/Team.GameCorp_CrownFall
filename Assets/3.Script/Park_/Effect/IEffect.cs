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
    float amount;
    float duration;

    public SlowEffect(float amount, float duration)
    {
        this.amount = amount;
        this.duration = duration;
    }

    public void Apply(PlayerController player)
    {

    }
}

public class DotEffect : IEffect
{
    float amount;
    float duration;

    public DotEffect(float amount, float duration)
    {
        this.amount = amount;
        this.duration = duration;
    }

    public void Apply(PlayerController player)
    {

    }
}