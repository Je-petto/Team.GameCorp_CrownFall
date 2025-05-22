using System.Collections;
using UnityEngine;

public class TowerSpawner : Spawner
{
    [Header("Event")]
    [SerializeField] private EventTowerSpawnBefore eventTowerSpawnBefore;
    [SerializeField] private EventTowerSpawnAfter eventTowerSpawnAfter;

    TowerControl tower;

    private void OnEnable()
    {
        eventTowerSpawnBefore?.Register(OnEventTowerSpawnBefore);
    }

    private void OnDisable()
    {
        eventTowerSpawnBefore?.Unregister(OnEventTowerSpawnBefore);
    }

    private void OnEventTowerSpawnBefore(EventTowerSpawnBefore e)
    {
        tower = Instantiate(e.towerControl);
        Quaternion look = Quaternion.LookRotation(spawnPoint.transform.forward);
        tower.transform.SetPositionAndRotation(spawnPoint.transform.position, look);

        tower.profile = towerProfile;
        tower.tag = towerProfile.teamTag;
        tower.state.Set(towerProfile);

        StartCoroutine(SpawnAfter_Co());
    }

    private IEnumerator SpawnAfter_Co()
    {
        yield return null;
        eventTowerSpawnAfter.towerControl = tower;
        eventTowerSpawnAfter?.Raise();
    }
}