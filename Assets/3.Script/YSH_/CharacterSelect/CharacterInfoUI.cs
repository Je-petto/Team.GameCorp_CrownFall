using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterInfoUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;

    public TMP_Text hpText;
    public TMP_Text attackText;
    public TMP_Text speedText;
    public TMP_Text rangeText;
    public TMP_Text intervalText;

    public Transform skillContent;
    public GameObject skillItemPrefab;

    public void SetCharacterInfo(CharacterInfo info)
    {
        nameText.text = info.characterName;
        descriptionText.text = info.description;

        hpText.text = $"HP: {info.hp}";
        attackText.text = $"ATK: {info.attack}";
        speedText.text = $"SPD: {info.speed}";
        rangeText.text = $"Range: {info.attackableRange}";
        intervalText.text = $"Interval: {info.attackInterval}";
    }
}