using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfoDatabase", menuName = "GameData/CharacterInfoDatabase")]
public class CharacterInfoDatabase : ScriptableObject
{
    public List<CharacterInfo> characterInfos;

    public CharacterInfo GetCharacterByID(int id)
    {
        if (id < 0 || id >= characterInfos.Count) return null;
        return characterInfos[id];
    }

    public CharacterInfo GetCharacterByCID(string cid)
    {
        return characterInfos.Find(info => info.cid == cid);
    }
}