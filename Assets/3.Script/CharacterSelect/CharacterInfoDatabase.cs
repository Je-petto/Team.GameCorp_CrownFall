using System.Collections.Generic;
using UnityEngine;

// 모든 캐릭터들의 정보를 모아놓은 데이터베이스
[CreateAssetMenu(fileName = "CharacterInfoDatabase", menuName = "GameData/CharacterInfoDatabase")]
public class CharacterInfoDatabase : ScriptableObject
{
    public List<CharacterInfo> characterList; // 캐릭터 정보 리스트
}

