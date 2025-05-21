using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventListener : MonoBehaviour
{
    public FXSpawnInfo[] fxSpawnInfos;
    //private CharacterControl owner;
    private Transform modelRoot;

    void Awake()
    {
        //TryGetComponent(out owner);
        modelRoot = transform.Find("root");
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

        PoolManager.I.Spawn(fx.prefab, pos, rot, null);
    }
}
