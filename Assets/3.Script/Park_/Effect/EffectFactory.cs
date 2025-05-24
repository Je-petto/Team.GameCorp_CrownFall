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


public static class SkillFactory
{
    public static ISkillAction CreateSkillAction(PlayerController caster, SkillData data)
    {
        switch (data.type)
        {
            case SkillType.FIRE: return new Skill_Fire(caster, data);
            case SkillType.HEAL: return new Skill_Heal(caster, data);
            case SkillType.FROST: return new Skill_Frost(caster, data);
        }
        return null;
    }   
}