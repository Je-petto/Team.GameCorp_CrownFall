using UnityEngine;

//public enum TowerType
//{
//    None = 0,
//    Red,
//    Blue,
//}

[CreateAssetMenu(menuName = "TowerProfile/Tower")]
public class TowerProfile : ScriptableObject
{
    [Header("PREFAB")]
    //public TowerType towertype = TowerType.None;
    public string teamTag;
    public GameObject towerModel;
    public GameObject shieldModel;

    [Header("ATTRIBUTE")]
    [Tooltip("타워 체력")] public int health;
    [Tooltip("보호막 체력")] public int shieldHealth;
}