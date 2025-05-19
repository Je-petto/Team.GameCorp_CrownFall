using System.Collections.Generic;

public enum EffectType
{
    EMPTY,
    DAMAGE,
}

public static class EffectFactory
{
    public static List<IEffect> CreateEffects(PlayerController player, List<EffectType> typeList)
    {
        List<IEffect> resEffects = new();
        
        foreach (var type in typeList)
        {
            switch (type)
            {
                case EffectType.DAMAGE:
                    {
                        resEffects.Add(new DamageEffect(player.attackDamage));
                        break;
                    }
            }
        }
        
        return resEffects;
    }
}