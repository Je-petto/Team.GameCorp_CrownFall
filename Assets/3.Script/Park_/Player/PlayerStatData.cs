using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Data/Player Data")]
public class PlayerStatData : ScriptableObject
{
    public float moveSpeed;
    public float rotateSpeed;
    public int attackDamage;
}