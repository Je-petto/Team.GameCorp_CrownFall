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
                        int index = data.type.Equals(SkillType.FROST) ? 0 : 2;
                        resEffects.Add(new SlowEffect(1f, data.duration, index));
                        break;
                    }
                case EffectType.DOT:
                    {
                        int index = data.type.Equals(SkillType.FIRE) ? 1 : 2;
                        resEffects.Add(new DotEffect(data.dot, data.duration, index));
                        break;
                    }
                case EffectType.HEAL:
                    {
                        resEffects.Add(new HealEffect(data.damage));
                        break;
                    }
            }
        }
        return resEffects;
    }
}


public static class SkillFactory
{
    public static ISkillAction CreateSkillAction(PlayerController_Net caster, SkillData data)
    {
        switch (data.type)
        {
            case SkillType.FIRE: return new Skill_Fire(caster, data);
            case SkillType.HEAL: return new Skill_Heal(caster, data);
            case SkillType.FROST: return new Skill_Frost(caster, data);
            case SkillType.LIGHTNING : return new Skill_YellowRod(caster, data);
        }
        return null;
    }   
}