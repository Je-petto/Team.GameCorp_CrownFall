public interface IEffect
{
    public abstract void Apply(PlayerController_Net player);
}

public class DamageEffect : IEffect
{
    private int amount;
    public DamageEffect(int amount){
        this.amount = amount;
    }
    public void Apply(PlayerController_Net player)
    {
        player.ApplyDamage(amount);
    }
}

public class SlowEffect : IEffect
{
    float amount;
    float duration;
    int index = -1;

    public SlowEffect(float amount, float duration, int index)
    {
        this.amount = amount;
        this.duration = duration;
        this.index = index;
    }

    public void Apply(PlayerController_Net player)
    {
        player.ApplySlow(duration, 50f, index);
    }
}

public class DotEffect : IEffect
{
    float amount;
    float duration;
    int index = -1;

    public DotEffect(float amount, float duration, int index)
    {
        this.amount = amount;
        this.duration = duration;
        this.index = index;
    }

    public void Apply(PlayerController_Net player)
    {
        player.ApplyDot(duration, amount, index);
    }
}

public class HealEffect : IEffect
{
    float amount;

    public HealEffect(float amount)
    {
        this.amount = amount;
    }

    public void Apply(PlayerController_Net player)
    {
        player.ApplyHeal(amount);
    }
}