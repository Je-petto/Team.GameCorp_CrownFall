using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    NONE,
    FROST,
    FIRE,
    LIGHTNING,
    LIGHTNINGRAY,
    HEAL
}

[CreateAssetMenu(menuName = "Data/SkillData")]
public class SkillData : ScriptableObject
{
    public SkillType type; // ��ų ����
    public string skillName;
    public string skillDescribe;
    public int damage; // ��ų ������
    public float duration; // ��ų ���ӽð�
    public float coolDown; // ��ų ��Ÿ��
    public float castingTime;               // SkillCasting
    public int dot; // ��ų ȿ�� ���ӽð�
    public GameObject castingMark;
    public GameObject prefab; // ��ų ��
    public float distance; // ���� �߻� �Ÿ�

    //��뿡�� �ִ� ȿ�� �÷���
    public List<EffectType> effectTypes;
}