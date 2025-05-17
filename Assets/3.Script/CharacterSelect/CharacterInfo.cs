using UnityEngine;

// 각 캐릭터의 정보를 담는 ScriptableObject
[CreateAssetMenu(fileName = "CharacterInfo", menuName = "GameData/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;  // 캐릭터 이름
    public int hp;                // 체력
    public int attack;            // 공격력
    public int speed;            // 이동 속도
    public string description;    // 캐릭터 설명
}
