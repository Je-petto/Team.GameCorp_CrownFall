using UnityEngine;

public enum CharacterType
{
    ShortRange,
    LongRange,
}

[CreateAssetMenu(fileName = "Character Profile", menuName = "Character/Character Profile")]
public class Playable : ScriptableObject
{
    private CharacterType characterType;

    public CharacterType Type => characterType;

    public GameObject model;

    public int damage;
    public int maxHp;
    public int moveSpeed;
}