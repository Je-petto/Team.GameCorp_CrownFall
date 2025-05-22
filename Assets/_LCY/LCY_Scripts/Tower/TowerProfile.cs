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
    [Tooltip("Ÿ�� ü��")] public int health;
    [Tooltip("��ȣ�� ü��")] public int shieldHealth;
}