using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameEvent/EventTowerSpawnAfter")]
public class EventTowerSpawnAfter : GameEvent<EventTowerSpawnAfter>
{
    public override EventTowerSpawnAfter Item => this;

    public TowerControl towerControl;
}