using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public FXSpawnInfo[] fxSpawnInfos;
    private Transform modelRoot;

    void Awake()
    {
        modelRoot = transform.Find("root"); // 필요 없으면 제거 가능
    }

    public void PlayFX(string fxName)
    {
        var fx = System.Array.Find(fxSpawnInfos, f => f.fxName == fxName);
        if (fx == null || fx.prefab == null || fx.spawnPoint == null)
        {
            Debug.LogWarning($"FX {fxName} 설정이 잘못됨");
            return;
        }

        Vector3 pos = fx.spawnPoint.position + fx.positionOffset;
        Quaternion rot = Quaternion.Euler(fx.rotationEuler);

        // Spawn은 PoolBehaviour를 리턴함
        PoolBehaviour poolObj = PoolManager.I.Spawn(fx.prefab, pos, rot, null);
        if (poolObj == null)
            return;

        // PoolableParticle을 찾아 duration 설정
        PoolableParticle particle = poolObj.GetComponent<PoolableParticle>();
        if (particle != null)
        {
            particle.SetDuration(fx.duration);
        }
    }
}
