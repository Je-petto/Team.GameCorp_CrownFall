using Unity.Collections;
using UnityEngine;
public enum CharacterType
{
    Melee,
    Tank,
    ADC,
    Support
}

// 각 캐릭터의 정보를 담는 ScriptableObject
[CreateAssetMenu(fileName = "CharacterInfo", menuName = "GameData/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public string characterName;    // 캐릭터 이름
    public string description;      // 캐릭터 설명
    public int hp;                  // 체력
    public int attack;              // 공격력
    public int speed;               // 이동 속도
    public float attackableRange;   // 공격 가능 범위

    [SerializeField] private CharacterType characterType;
    public CharacterType Type => characterType;

    // Edit from Park
    [Header("Model")]
    public GameObject model;
    public RuntimeAnimatorController selectorAnimator;
    public RuntimeAnimatorController inGameAnimator;


    [ReadOnly] public float rotateSpeed = 720f;                       // 회전 속도
}
