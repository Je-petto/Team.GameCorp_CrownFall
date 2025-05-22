using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : MonoBehaviour
{
    [Header("Event")]
    public EventTowerSpawnBefore eventTowerASpawnBefore;
    public EventTowerSpawnBefore eventTowerBSpawnBefore;

    private IEnumerator Start()
    {
        yield return null;
        eventTowerASpawnBefore.Raise();
        eventTowerBSpawnBefore.Raise();
    }
}