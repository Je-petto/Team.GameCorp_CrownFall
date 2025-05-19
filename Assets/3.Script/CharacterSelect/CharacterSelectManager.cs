using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterSelectManager : MonoBehaviour
{
    [Header("캐릭터 오브젝트 관련")]
    public List<GameObject> characterList; // 캐릭터 프리팹 리스트

    [Header("프리팹 관련")]
    public List<GameObject> characterPrefabList;

    [Header("캐릭터 위치 관련")]
    public Transform position0; // 왼쪽
    public Transform position1; // 가운데 (선택된 캐릭터)
    public Transform position2; // 오른쪽

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
            GameObject character = Instantiate(characterPrefabList[i]); // 또는 characterPrefabList[i]
            character.SetActive(false); // 처음엔 안 보이게
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
    // 모든 캐릭터를 우선 비활성화해요
    foreach (GameObject character in characterList)
    {
        character.SetActive(false);
    }

    // 인덱스 계산 (순환 구조)
    int leftIndex = (currentIndex - 1 + characterList.Count) % characterList.Count;
    int rightIndex = (currentIndex + 1) % characterList.Count;

    // 왼쪽 캐릭터 표시 및 위치 설정
    characterList[leftIndex].SetActive(true);
    characterList[leftIndex].transform.position = position0.position;
    characterList[leftIndex].transform.rotation = Quaternion.Euler(0, 180, 0);

    // 가운데 캐릭터 (선택된 캐릭터)
    characterList[currentIndex].SetActive(true);
    characterList[currentIndex].transform.position = position1.position;
    characterList[currentIndex].transform.rotation = Quaternion.Euler(0, 180, 0);

    // 오른쪽 캐릭터 표시 및 위치 설정
    characterList[rightIndex].SetActive(true);
    characterList[rightIndex].transform.position = position2.position;
    characterList[rightIndex].transform.rotation = Quaternion.Euler(0, 180, 0);
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

