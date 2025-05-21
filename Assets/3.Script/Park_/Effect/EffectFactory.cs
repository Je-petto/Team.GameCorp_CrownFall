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
            case SkillType.RED: return new Skill_Red(caster, data);
            case SkillType.GREEN: return new Skill_Green(caster, data);
            case SkillType.BLUE: return new Skill_Blue(caster, data);
        }
        return null;
    }   
}