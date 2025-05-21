using System.Collections.Generic;
using UnityEngine;

public class CharacterInfoDatabase : ScriptableObject
{
    public List<CharacterInfo> characterInfoList;

    public CharacterInfo GetCharacterByID(int id)
    {
        if (id < 0 || id >= characterInfoList.Count)
        {
            Debug.LogError($"Invalid character ID: {id}");
            return null;
        }
        return characterInfoList[id];
    }
}
