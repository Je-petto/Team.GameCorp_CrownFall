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
    public int dot; // 스킬 효과 지속시간
    public GameObject prefab; // 스킬 모델
    public float distance; // 레이 발사 거리

    //상대에게 주는 효과 플래그
    public List<EffectType> effects; 
}