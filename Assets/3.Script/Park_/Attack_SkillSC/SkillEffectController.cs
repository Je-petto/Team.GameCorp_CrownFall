using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using DG.Tweening;
using UnityEngine;

public class SkillEffectController : MonoBehaviour
{
    public PlayerController_Net caster;
    private ParticleSystem ps;
    List<IEffect> effects;

    public SkillData data;

    public void SetProps(PlayerController_Net caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;

        effects = EffectFactory.CreateSkillEffects(data);
    }

    void OnEnable()
    {
        // 캐릭터 움직임 잠금
        ps.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (data.type.Equals(SkillType.HEAL))
        {
            ApplyHeal(other);
        }
        else
        {
            ApplyDamage(other);
        }
    }

    private void ApplyHeal(Collider other)
    {
        var target = other.GetComponent<PlayerController_Net>();
        if (target == null) return;

        // 다른팀이면 무시.
        if (target.teamCode != caster.teamCode) return;

        if (other.GetComponent<TowerControl>()) return;         //타워는 스킬 피해를 받지 않음.

        Debug.Log("힐 적중!!");

        PlayerController_Net enemy = other.GetComponent<PlayerController_Net>();

        foreach (var e in effects)
        {
            e.Apply(enemy);
        }
    }

    private void ApplyDamage(Collider other)
    {
        var target = other.GetComponent<PlayerController_Net>();
        if (target == null) return;

        // 같은 팀이면 무시.
        if (target.teamCode == caster.teamCode) return;

        if (other.GetComponent<TowerControl>()) return;         //타워는 스킬 피해를 받지 않음.

        Debug.Log("데미지 적중!!");

        PlayerController_Net enemy = other.GetComponent<PlayerController_Net>();

        foreach (var e in effects)
        {
            e.Apply(enemy);
        }
    }
}