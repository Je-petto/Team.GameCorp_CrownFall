using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
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
        ps.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.GetComponent<PlayerController>();
        if (target == null) return;

        // 같은 팀이면 무시.
        if (target.teamCode == caster.teamCode) return;

        if (other.GetComponent<PlayerController>()) return;         //타워는 스킬 피해를 받지 않음.

        Debug.Log("스킬 적중!!");

        PlayerController enemy = other.GetComponent<PlayerController>();

        foreach (var e in effects)
        {
            e.Apply(enemy);
        }
    }
}