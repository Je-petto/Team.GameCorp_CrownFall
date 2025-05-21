using System.Collections.Generic;

public enum EffectType
{
    NONE,
    DAMAGE,
    HEAL,
    CLEAR,
    SLOW,
    DOT,
}

public static class EffectFactory
{
    public static List<IEffect> CreateSkillEffects(SkillData data)
    {
        List<IEffect> resEffects = new();
        
        foreach (var type in data.effectTypes)
        {
            switch (type)
            {
                case EffectType.DAMAGE:
                {
                    resEffects.Add(new DamageEffect((int)data.damage));
                    break;
                }
                case EffectType.SLOW:
                {
                    resEffects.Add(new SlowEffect(1f, data.duration));
                    break;
                }
                case EffectType.DOT:
                {
                    resEffects.Add(new DotEffect(1f, data.duration));
                    break;
                }
            }
        }
        return resEffects;
    }
}