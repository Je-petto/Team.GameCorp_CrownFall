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
    [Tooltip("��ų Ÿ��")] public SkillType type;
    [Tooltip("��ų ��")] public GameObject prefab;
    [Tooltip("���� ������ �ð�")] public int dot;
    [Tooltip("��ų ������")] public float damage;
    [Tooltip("��ų ���� �ð�")] public float duration;
    [Tooltip("��ų ��Ÿ��")] public float cool;
}