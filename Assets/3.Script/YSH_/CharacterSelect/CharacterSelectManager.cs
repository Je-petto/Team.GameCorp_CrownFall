using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("생성 위치 리스트")]
    [Tooltip("순서대로 Left, Center, Right(필요시 Offscreen 추가)")]
    public List<Transform> spawnPositions;

    [Header("캐릭터 리스트(내부용)")]
    private List<GameObject> characterList;

    [Header("UI 관련")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI speedText;

    [Header("캐릭터 정보 데이터")]
    public CharacterInfoDatabase characterDatabase;

    private int currentIndex = 0;

    void Start()
    {
        characterList = new List<GameObject>();

        for (int i = 0; i < characterDatabase.characterInfoList.Count; i++)
        {
            CharacterInfo info = characterDatabase.characterInfoList[i];
            if (info.model == null)
            {
                Debug.LogError($"CharacterInfo {i}의 model 프리팹이 비어 있습니다.");
                continue;
            }

            int spawnIdx = i % spawnPositions.Count;
            Transform spawnPoint = spawnPositions[spawnIdx];

            GameObject character = Instantiate(info.model, spawnPoint.position, spawnPoint.rotation);

            // 캐릭터 정보 할당
            Character characterComponent = character.GetComponent<Character>();
            if (characterComponent == null)
                characterComponent = character.AddComponent<Character>();

            characterComponent.characterInfo = info;

            character.SetActive(false);
            characterList.Add(character);
        }

        currentIndex = 0;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    public void OnClickLeft()
    {
        currentIndex = (currentIndex - 1 + characterList.Count) % characterList.Count;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    public void OnClickRight()
    {
        currentIndex = (currentIndex + 1) % characterList.Count;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    void UpdateCharacterPositions()
    {
        foreach (GameObject character in characterList)
            character.SetActive(false);

        int leftIndex = (currentIndex - 1 + characterList.Count) % characterList.Count;
        int rightIndex = (currentIndex + 1) % characterList.Count;

        GameObject leftChar = characterList[leftIndex];
        GameObject centerChar = characterList[currentIndex];
        GameObject rightChar = characterList[rightIndex];

        leftChar.SetActive(true);
        centerChar.SetActive(true);
        rightChar.SetActive(true);

        float duration = 0.3f;

        leftChar.transform.DOMove(spawnPositions[3].position, duration);
        centerChar.transform.DOMove(spawnPositions[0].position, duration);
        rightChar.transform.DOMove(spawnPositions[1].position, duration);

        leftChar.transform.rotation = Quaternion.Euler(0, 180, 0);
        centerChar.transform.rotation = Quaternion.Euler(0, 180, 0);
        rightChar.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    void UpdateCharacterInfoUI()
    {
        Character selectedCharacter = characterList[currentIndex].GetComponent<Character>();

        if (selectedCharacter == null || selectedCharacter.characterInfo == null)
        {
            Debug.LogError("선택된 캐릭터에 CharacterInfo가 없습니다.");
            return;
        }

        CharacterInfo info = selectedCharacter.characterInfo;

        nameText.text = info.characterName;
        hpText.text = "HP: " + info.hp.ToString();
        attackText.text = "Attack: " + info.attack.ToString();
        speedText.text = "Speed: " + info.speed.ToString();
        descriptionText.text = info.description;
    }

    public void LoadCharacterPrefab(CharacterInfo info)
    {
        GameObject player = GameObject.Find("Player");
        if (player == null)
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        Transform modelRoot = player.transform.Find("_Model");
        if (modelRoot == null)
        {
            Debug.LogError("Player 하위에 '_Model' 오브젝트가 없습니다.");
            return;
        }

        foreach (Transform child in modelRoot)
            Destroy(child.gameObject);

        GameObject modelInstance = Instantiate(info.model, modelRoot);
        modelInstance.transform.localPosition = Vector3.zero;
        modelInstance.transform.localRotation = Quaternion.identity;

        Character characterComponent = player.GetComponent<Character>();
        if (characterComponent != null)
            characterComponent.characterInfo = info;

        CharacterEffectController effect = player.GetComponent<CharacterEffectController>();
        if (effect != null)
            effect.modelRoot = modelRoot;

        Debug.Log("선택된 캐릭터 프리팹이 Player에 로드되었습니다.");
    }

    public void OnClickStart()
    {
        Character character = characterList[currentIndex].GetComponent<Character>();
        if (character != null && character.characterInfo != null)
        {
            CharacterSelectionData.selectedCharacterIndex = currentIndex;
            CharacterSelectionData.selectedCharacterInfo = character.characterInfo;
        }

        SceneManager.LoadScene("SHInGameScene");
    }
}