using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public Transform[] redTeamSpawns;
    public Transform[] blueTeamSpawns;

    void Awake()
    {
        Instance = this;
    }

    public Vector3 GetSpawnPoint(int teamCode)
    {
        Transform[] spawnPoints = teamCode == 0 ? redTeamSpawns : blueTeamSpawns;
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}
