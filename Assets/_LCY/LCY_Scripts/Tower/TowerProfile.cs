using UnityEngine;
using CustomInspector;

[CreateAssetMenu(menuName = "TowerProfile/Tower")]
public class TowerProfile : ScriptableObject
{
    [HorizontalLine("TAG"), HideField] public bool b0;
    [Tooltip("팀 태그")] public string teamTag;

    [SerializeField, HorizontalLine("TOWER"), HideField] public bool b1;
    [Tooltip("타워 체력")] public int health;
    [Tooltip("타워 모델")] public GameObject towerModel;

    [SerializeField, HorizontalLine("SHIELD"), HideField] public bool b2;
    [Tooltip("보호막 체력")] public int shieldHealth;
    [Tooltip("보호막 모델")] public GameObject shieldModel;
    [Tooltip("보호막 효과")] public ParticleSystem shieldParticle;
}