using UnityEngine;
using CustomInspector;

[CreateAssetMenu(menuName = "TowerProfile/Tower")]
public class TowerProfile : ScriptableObject
{
    [HorizontalLine("TAG"), HideField] public bool b0;
    [Tooltip("�� �±�")] public int teamCode;

    [SerializeField, HorizontalLine("TOWER"), HideField] public bool b1;
    [Tooltip("Ÿ�� ü��")] public int health;
    [Tooltip("Ÿ�� ��")] public GameObject towerModel;

    [SerializeField, HorizontalLine("SHIELD"), HideField] public bool b2;
    [Tooltip("��ȣ�� ü��")] public int shieldHealth;
    [Tooltip("��ȣ�� ��")] public GameObject shieldModel;
    [Tooltip("��ȣ�� ȿ��")] public ParticleSystem shieldParticle;
}