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
    public SkillType type; // ��ų ����
    public float damage; // ��ų ������
    public float duration; // ��ų ���ӽð�
    public float coolDown; // ��ų ��Ÿ��
    public int effectDuration; // ��ų ȿ�� ���ӽð�
    public GameObject model; // ��ų ��
}