using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{

    [Header("프리팹 관련")]
    public List<GameObject> characterPrefabList;

    [Header("생성 위치 리스트")]
    [Tooltip("순서대로 Left, Center, Right(필요시 Offscreen 추가)")]
    public List<Transform> spawnPositions;    

    [Header("캐릭터 리스트(내부용)")]
    private List<GameObject> characterList; // 캐릭터 프리팹 리스트

    [Header("UI 관련")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI speedText;

    [Header("캐릭터 정보 데이터")]
    public CharacterInfoDatabase characterDatabase; // ScriptableObject로 만든 데이터베이스

    private int currentIndex = 0; // 현재 선택된 캐릭터 인덱스

    void Start()
    {
        characterList = new List<GameObject>();

        // 캐릭터 데이터 리스트 수만큼 생성
        for (int i = 0; i < characterPrefabList.Count; i++)
        {
            // prefabCount > spawnPositions.Count 여도 에러 안 나게 모듈로 처리
            int spawnIdx = i % spawnPositions.Count;
            Transform spawnPoint = spawnPositions[spawnIdx];

            // 생성 즉시 원하는 위치·회전으로 스폰
            GameObject character = Instantiate(
                characterPrefabList[i],
                spawnPoint.position,
                spawnPoint.rotation
            );
            character.SetActive(false);
            characterList.Add(character);
        }

        currentIndex = 0;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    // 왼쪽 버튼 클릭 시 실행되는 함수
    public void OnClickLeft()
    {
        currentIndex = (currentIndex - 1 + characterList.Count) % characterList.Count;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    // 오른쪽 버튼 클릭 시 실행되는 함수
    public void OnClickRight()
    {
        currentIndex = (currentIndex + 1) % characterList.Count;
        UpdateCharacterPositions();
        UpdateCharacterInfoUI();
    }

    // 캐릭터 위치를 업데이트하는 함수
    void UpdateCharacterPositions()
    {
        foreach (GameObject character in characterList)
        {
            character.SetActive(false);
        }

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

        // 선택적으로 회전도 줄 수 있어요
        leftChar.transform.rotation = Quaternion.Euler(0, 180, 0);
        centerChar.transform.rotation = Quaternion.Euler(0, 180, 0);
        rightChar.transform.rotation = Quaternion.Euler(0, 180, 0);
    }

    // 선택된 캐릭터의 정보를 UI에 표시하는 함수
void UpdateCharacterInfoUI()
{
    // 현재 선택된 캐릭터 오브젝트에서 Character 컴포넌트를 가져와요
    Character selectedCharacter = characterList[currentIndex].GetComponent<Character>();
    
    if (selectedCharacter == null || selectedCharacter.characterInfo == null)
    {
        Debug.LogError("선택된 캐릭터에 CharacterInfo가 없어요!");
        return;
    }

    CharacterInfo info = selectedCharacter.characterInfo;

    nameText.text = info.characterName;
    hpText.text = "HP: " + info.hp.ToString();
    attackText.text = "Attack: " + info.attack.ToString();
    speedText.text = "Speed: " + info.speed.ToString();
    descriptionText.text = info.description;
}

    // 시작 버튼을 누르면 인게임 씬으로 넘어가는 함수
    public void OnClickStart()
    {
        CharacterSelectionData.selectedCharacterIndex = currentIndex;
        SceneManager.LoadScene("SHInGameScene");
    }
}

