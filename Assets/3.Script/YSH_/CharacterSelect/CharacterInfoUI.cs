using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    [Header("기본 정보")]
    [SerializeField] private TextMeshProUGUI characterNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI typeText;

    [Header("스탯 텍스트 (개별 연결)")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private TextMeshProUGUI rangeText;
    [SerializeField] private TextMeshProUGUI intervalText;

    [Header("스킬 UI")]
    [SerializeField] private TextMeshProUGUI skillTypeText;
    [SerializeField] private TextMeshProUGUI skillDamageText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private TextMeshProUGUI skillCooldownText;

    /// <summary>
    /// 캐릭터 정보를 UI에 표시
    /// </summary>
    /// 
    private void Awake()
    {
        // 씬 시작 시 UI 비활성화
        gameObject.SetActive(false);
    }

    public void SetCharacterInfo(CharacterInfo info)
    {
        if (info == null)
        {
            Debug.LogWarning("CharacterInfo is null!");
            return;
        }

        if (!gameObject.activeSelf)
            gameObject.SetActive(true);

        Debug.Log($"Setting info for character: {info.characterName}");

        // 기본 정보
        characterNameText.text = info.characterName;
        descriptionText.text = info.description;
        typeText.text = info.Type.ToString();

        // 스탯 정보
        hpText.text = info.hp.ToString();
        attackText.text = info.attack.ToString();
        defenseText.text = info.defense.ToString();
        speedText.text = info.speed.ToString();
        rangeText.text = info.attackableRange.ToString("0.0");
        intervalText.text = info.attackInterval.ToString("0.00");

        //스킬 정보만 표시
        if (info.skillSet != null && info.skillSet.Count > 0)
        {
            SkillData skill = info.skillSet[0];
            skillTypeText.text = $"{skill.type}";
            skillDamageText.text = $"Damage: {skill.damage}";
            skillCooldownText.text = $"Cooldown: {skill.coolDown}s";
        }
        else
        {
            skillTypeText.text = "Type: -";
            skillDamageText.text = "Damage: -";
            skillCooldownText.text = "Cooldown: -";
        }
    }
}