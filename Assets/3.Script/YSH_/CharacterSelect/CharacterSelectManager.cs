using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterSelectManager : MonoBehaviour
{
    public static CharacterSelectManager Instance { get; private set; }

    [SerializeField] private CharacterInfoDatabase characterDatabase;
    [SerializeField] private CharacterInfoUI characterInfoUI;
    [SerializeField] private Transform spawnPoint;

    private GameObject currentCharacterModel;

    public CharacterInfo SelectedCharacterInfo { get; private set; }

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }

    public void SelectCharacterByCID(string cid)
    {
        var info = characterDatabase.GetCharacterByCID(cid);
        if (info == null)
        {
            Debug.LogError($"해당 CID({cid})를 가진 캐릭터를 찾을 수 없습니다.");
            return;
        }

        SelectedCharacterInfo = info;

        if (currentCharacterModel != null)
            Destroy(currentCharacterModel);

        currentCharacterModel = Instantiate(info.model, spawnPoint.position, spawnPoint.rotation);

        characterInfoUI?.SetCharacterInfo(info);
    }

    public void SaveSelectedCharacterCID()
    {
        if (SelectedCharacterInfo != null)
        {
            PlayerPrefs.SetString("SelectedCharacterCID", SelectedCharacterInfo.cid);
            PlayerPrefs.Save();
        }
    }

    public void OnStartGame()
    {
        SaveSelectedCharacterCID();
        UnityEngine.SceneManagement.SceneManager.LoadScene("InGameScene");
    }
}