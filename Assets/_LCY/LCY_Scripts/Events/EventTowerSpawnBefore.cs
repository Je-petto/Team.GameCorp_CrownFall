using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent/EventTowerSpawnBefore")]
public class EventTowerSpawnBefore : GameEvent<EventTowerSpawnBefore>
{
    public override EventTowerSpawnBefore Item => this;

    public TowerControl towerControl;
}