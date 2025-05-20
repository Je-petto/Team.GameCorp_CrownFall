using UnityEngine;

public enum SkillType
{
    NONE,
    RED,
    GREEN,
    BLUE,
    YELLOW
}

[CreateAssetMenu(menuName = "SkillData/Skill", fileName = "Skill")]
public class SkillData : ScriptableObject
{
    [Tooltip("스킬 타입")] public SkillType type;
    [Tooltip("스킬 모델")] public GameObject prefab;
    [Tooltip("지속 데미지 시간")] public int dot;
    [Tooltip("스킬 데미지")] public float damage;
    [Tooltip("스킬 지속 시간")] public float duration;
    [Tooltip("스킬 쿨타임")] public float cool;
}