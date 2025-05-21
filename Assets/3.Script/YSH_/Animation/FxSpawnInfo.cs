using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FXSpawnInfo
{
    [Tooltip("애니메이션 이벤트 함수에서 호출할 이름입니다. 예: summon, magic")]
    public string fxName;

    [Tooltip("생성할 FX 프리팹 (PoolableParticle)")]
    public PoolableParticle prefab;

    [Tooltip("FX가 소환될 기준 위치 (예: _fxcore, rhand 등)")]
    public Transform spawnPoint;

    [Tooltip("SpawnPoint로부터의 위치 오프셋")]
    public Vector3 positionOffset;

    [Tooltip("FX의 회전값 (Euler 각도)")]
    public Vector3 rotationEuler;
}