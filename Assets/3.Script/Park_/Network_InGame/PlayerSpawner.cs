using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : BehaviourSingleton<PlayerSpawner>
{
    protected override bool IsDontdestroy() => false;

    [SerializeField] List<CharacterInfo> characterList;
    private Dictionary<string, CharacterInfo> characterDic = new();

    [Header("Selector")]
    public string selectCId;

    private void Awake()
    {
        if (characterDic.Count == 0 && characterList.Count > 0)
        {
            foreach (var p in characterList)
            {
                characterDic[p.cid] = p;
            }
            Debug.Log("[PlayerSpawner] characterDic 초기화 완료");
        }
        else
        {
            Debug.LogWarning("[PlayerSpawner] characterList가 비어 있거나 이미 초기화됨");
        }
    }
    
    public CharacterInfo GetCharacterInfo(string cid)
    {
        if (!characterDic.TryGetValue(cid, out var info))
        {
            Debug.LogError($"[PlayerSpawner] 캐릭터 정보를 찾을 수 없습니다: {cid}");
            return null;
        }
        return info;
    }
}