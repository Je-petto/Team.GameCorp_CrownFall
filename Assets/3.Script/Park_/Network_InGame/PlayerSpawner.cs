using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] List<CharacterInfo> characterList;
    private Dictionary<string, CharacterInfo> characterDic = new();

    [SerializeField] GameObject playerBase;               // 캐릭터 베이스
    [Header("Selector")]
    public string selectCId;

    void Start()
    {
        foreach (var p in characterList)
        {
            characterDic.Add(p.cid, p);
        }

        StartCoroutine(SpawnPlayer_Co());
    }

    IEnumerator SpawnPlayer_Co()
    {
        yield return new WaitForEndOfFrame();
        PlayerController player = Instantiate(playerBase).GetComponent<PlayerController>();
        Debug.Log($"{characterDic.Count}");
        
        //대기창에서 선택한 플레이어 data 가져오기

        //네트워크용
        /* 
            CharacterData data = NetworkClient.connection.identity.GetComponent<NetworkPlayer>().selectedCharacter;
            player.SetCharacter(data);

        */
        //테스트용
        player.SetCharacter(characterDic[selectCId]);
    }
}