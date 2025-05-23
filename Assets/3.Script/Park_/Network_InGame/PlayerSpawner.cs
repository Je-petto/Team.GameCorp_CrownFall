using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : BehaviourSingleton<PlayerSpawner>
{
    protected override bool IsDontdestroy() => false;

    [SerializeField] List<CharacterInfo> characterList;
    private Dictionary<string, CharacterInfo> characterDic = new();

    [Header("Selector")]
    public string selectCId;

    void Start()
    {
        foreach (var p in characterList)
        {
            characterDic.Add(p.cid, p);
        }
    }

    public CharacterInfo GetCharacterInfo(string cid = "0")
    {
        
        return characterDic[cid];
    }
}