using System.Collections.Generic;
using UnityEngine;

public class CharacterSelector : MonoBehaviour
{
    [SerializeField] List<CharacterInfo> characterList;
    private Dictionary<CharacterType, CharacterInfo> characterDic = new();

    [Header("Selector")]
    public CharacterType selectType;

    void Start()
    {
        foreach (var p in characterList)
        {
            characterDic.Add(p.Type, p);
        }

    }
}