using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class SkillEffectController : NetworkBehaviour
{
    public PlayerController_Net caster;
    private ParticleSystem ps;
    List<IEffect> effects = new();

    public void SetProps(PlayerController_Net caster, List<IEffect> effects)
    {
        this.caster = caster;
        this.effects = effects;
    }

    void OnEnable()
    {
        // 캐릭터 움직임 잠금
        ps.Play();
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out TowerControl tower)) return;

        foreach (var e in effects)
        {
            if (e is HealEffect)
            {
                ApplyHeal(other);
            }
            else
            {
                ApplyDamage(other);
            }
        }
    }

    private void ApplyHeal(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController_Net target) || target.teamCode != caster.teamCode) return;

        Debug.Log("Applyheal!");

        foreach (var e in effects)
        {
            e.Apply(target);
        }
    }

    private void ApplyDamage(Collider other)
    {
        if (!other.TryGetComponent(out PlayerController_Net target) || target.teamCode == caster.teamCode) return;

        Debug.Log("A");

        foreach (var e in effects)
        {
            e.Apply(target);
        }
    }
}