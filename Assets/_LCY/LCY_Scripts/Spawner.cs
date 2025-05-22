using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
    [SerializeField] protected Transform spawnPoint;
    [SerializeField] protected TowerProfile towerProfile;
}