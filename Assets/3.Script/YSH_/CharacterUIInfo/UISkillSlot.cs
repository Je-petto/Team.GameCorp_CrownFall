using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UISkillSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI typeText;
    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private TextMeshProUGUI cooldownText;

    /// <summary>
    /// SkillData 데이터로 UI 업데이트
    /// </summary>
    public void SetSkillInfo(SkillData skill)
    {
        typeText.text = $"Type: {skill.type}";
        damageText.text = $"Damage: {skill.damage}";
        cooldownText.text = $"Cooldown: {skill.coolDown}s";
    }
}

