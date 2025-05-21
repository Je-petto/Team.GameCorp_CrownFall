using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }

    [SerializeField] private CharacterInfoDatabase characterDatabase;
    [SerializeField] private Transform spawnPoint;

    private GameObject currentCharacterModel;

    public CharacterInfo SelectedCharacterInfo { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void SelectCharacter(int id)
    {
        var info = characterDatabase.GetCharacterByID(id);
        if (info == null) return;

        SelectedCharacterInfo = info;

        // 기존 캐릭터 모델 제거
        if (currentCharacterModel != null)
            Destroy(currentCharacterModel);

        // 새로운 캐릭터 모델 생성 (spawnPoint의 회전을 사용)
        currentCharacterModel = Instantiate(info.model, spawnPoint.position, spawnPoint.rotation);

        // UI에 캐릭터 정보 전달
        CharacterInfoUI ui = FindObjectOfType<CharacterInfoUI>();
        if (ui != null)
            ui.SetCharacterInfo(info);
    }
}

    // [Header("기본 정보")]
    // public TMP_Text nameText;
    // public TMP_Text descriptionText;
    // public TMP_Text typeText;

    // [Header("스탯 정보")]
    // public TMP_Text hpText;
    // public TMP_Text attackText;
    // public TMP_Text speedText;
    // public TMP_Text rangeText;
    // public TMP_Text intervalText;
