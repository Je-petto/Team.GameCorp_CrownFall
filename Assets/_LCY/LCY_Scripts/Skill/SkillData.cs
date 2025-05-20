using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    NONE,
    RED,
    GREEN,
    BLUE,
    YELLOW
}

[CreateAssetMenu(menuName = "Data/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillType type; // 스킬 종류
    public float damage; // 스킬 데미지
    public float duration; // 스킬 지속시간
    public float coolDown; // 스킬 쿨타임
    public int effectDuration; // 스킬 효과 지속시간
    public GameObject model; // 스킬 모델
}