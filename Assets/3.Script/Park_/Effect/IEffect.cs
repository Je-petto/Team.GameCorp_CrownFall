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
        player.effectHandler.ApplyDamage(amount);
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
        player.effectHandler.ApplySlow(duration, amount);
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
        player.effectHandler.ApplyDot(duration, amount);
    }
}