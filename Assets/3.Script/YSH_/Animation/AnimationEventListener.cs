using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public FXSpawnInfo[] fxSpawnInfos;
    private Transform modelRoot;

    void Awake()
    {
        modelRoot = transform.Find("root"); // 필요 없다면 제거 가능
    }

    public void PlayFX(string fxName)
    {
        var fx = System.Array.Find(fxSpawnInfos, f => f.fxName == fxName);
        if (fx == null || fx.prefab == null || fx.spawnPoint == null)
        {
            Debug.LogWarning($"FX {fxName} 설정이 잘못됨");
            return;
        }

        // 항상 새로 Spawn – Despawn 중 여부를 무시
        PoolBehaviour poolObj = PoolManager.I.ForceSpawn(fx.prefab);
        if (poolObj == null || poolObj.gameObject == null)
        {
            Debug.LogWarning($"FX {fxName} 생성 실패 또는 오브젝트 손상");
            return;
        }

        // 부모를 spawnPoint로 설정하면 FX가 리깅 위치를 따라감
        Transform fxTransform = poolObj.transform;
        if (fxTransform == null)
        {
            Debug.LogWarning($"FX {fxName}의 transform 손상");
            return;
        }

        fxTransform.SetParent(fx.spawnPoint, false);
        fxTransform.localPosition = fx.positionOffset;
        fxTransform.localRotation = Quaternion.Euler(fx.rotationEuler);

        // FX 지속 시간 적용
        var particle = poolObj.GetComponent<PoolableParticle>();
        if (particle != null && particle.gameObject != null)
        {
            particle.SetDuration(fx.duration);
        }
    }
}

