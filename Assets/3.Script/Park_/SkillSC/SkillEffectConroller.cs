using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillEffectConroller : MonoBehaviour
{
    public PlayerController caster;
    private ParticleSystem ps;
    List<IEffect> effects;

    public SkillData data;

    public void SetProps(PlayerController caster, SkillData data)
    {
        this.caster = caster;
        this.data = data;
    }

    void OnEnable()
    {
        // 캐릭터 움직임 잠금
        // LockMovementForDuration(1f);
        ps.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<PlayerController>();
        if (target == null) return;

        // 적 판정
        // if (!target.teamData.IsEnemy(caster.teamData)) return;

        Debug.Log("스킬 적중!!");

        PlayerController enemy = other.GetComponent<PlayerController>();

        foreach (var e in effects)
        {
            e.Apply(enemy);
        }
    }
}