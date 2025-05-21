using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfoDatabase", menuName = "Character Info Database")]
public class CharacterInfoDatabase : ScriptableObject
{
    public List<CharacterInfo> characterInfoList;
}