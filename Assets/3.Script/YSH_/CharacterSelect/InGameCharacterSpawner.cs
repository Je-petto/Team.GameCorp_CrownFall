using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCharacterSpawner : MonoBehaviour
{
    [SerializeField] private CharacterInfoDatabase characterDatabase;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] GameObject playerBase;               // 캐릭터 베이스
    void Start()
    {
        string selectedCID = PlayerPrefs.GetString("SelectedCharacterCID", "");

        if (string.IsNullOrEmpty(selectedCID))
        {
            Debug.LogError("선택된 캐릭터 CID가 없습니다!");
            return;
        }

        var info = characterDatabase.GetCharacterByCID(selectedCID);
        if (info == null)
        {
            Debug.LogError($"CID({selectedCID})에 해당하는 캐릭터 정보를 찾을 수 없습니다.");
            return;
        }

        StartCoroutine(SpawnPlayer_Co(info));

        // GameObject character = Instantiate(info.model, spawnPoint.position, spawnPoint.rotation);
        // Animator animator = character.GetComponent<Animator>();
        // if (animator != null && info.inGameAnimator != null)
        // {
        //     animator.runtimeAnimatorController = info.inGameAnimator;
        // }

        // 필요한 컴포넌트 설정도 이곳에서 추가 가능
    }

    
    IEnumerator SpawnPlayer_Co(CharacterInfo info)
    {
        yield return new WaitForEndOfFrame();
        PlayerController player = Instantiate(playerBase).GetComponent<PlayerController>();
  
        player.SetCharacter(info);
    }
}
