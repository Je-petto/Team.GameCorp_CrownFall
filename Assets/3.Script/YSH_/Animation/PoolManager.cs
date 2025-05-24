using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : BehaviourSingleton<PoolManager>
{
    protected override bool IsDontdestroy() => false;

    private Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>> prefabs;
    private Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>> instances;

    protected override void Awake()
    {
        base.Awake();
        prefabs = new Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>>();
        instances = new Dictionary<PoolBehaviour, ObjectPool<PoolBehaviour>>();
    }

    public void ClearPool()
    {
        foreach (var p in prefabs) p.Value.Clear();
        foreach (var i in instances) i.Value.Clear();
        prefabs.Clear();
        instances.Clear();
    }

    public void WarmPool(PoolBehaviour pb, int size = 10)
    {
        if (prefabs.ContainsKey(pb))
        {
            Debug.LogWarning($"이미 생성한 프리팹 : {pb.name}");
            return;
        }

        var pool = new ObjectPool<PoolBehaviour>(
            createFunc: () =>
            {
                PoolBehaviour p = Instantiate(pb);
                p.poolmanager = this;
                return p;
            },
            actionOnGet: (v) =>
            {
                if (v != null && v.gameObject != null)
                    v.gameObject.SetActive(true);
            },
            actionOnRelease: (v) =>
            {
                if (v != null && v.gameObject != null)
                    v.gameObject.SetActive(false);
            },
            actionOnDestroy: null,
            maxSize: size
        );

        prefabs[pb] = pool;

        for (int i = 0; i < size; i++)
        {
            var obj = pool.Get();
            if (obj != null && obj.gameObject != null)
                pool.Release(obj);
        }
    }

    public PoolBehaviour Spawn(PoolBehaviour pb, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if (!prefabs.ContainsKey(pb))
            WarmPool(pb);

        var pool = prefabs[pb];
        var clone = pool.Get();

        clone.poolmanager = this; // 안전하게 다시 설정
        clone.transform.SetParent(parent ?? transform, true);
        clone.transform.position = pos;
        clone.transform.rotation = rot;

        instances[clone] = pool;
        return clone;
    }

    public PoolBehaviour ForceSpawn(PoolBehaviour pb)
    {
        if (!prefabs.ContainsKey(pb))
            WarmPool(pb);

        var pool = prefabs[pb];
        var clone = pool.Get();

        if (clone == null || clone.gameObject == null)
        {
            Debug.LogWarning("ForceSpawn 실패: pool 손상 감지 – WarmPool 재시도");
            WarmPool(pb);
            pool = prefabs[pb];
            clone = pool.Get();

            if (clone == null || clone.gameObject == null)
            {
                Debug.LogError("ForceSpawn 실패: 여전히 손상됨");
                return null;
            }
        }

        clone.poolmanager = this;
        instances[clone] = pool;
        clone.gameObject.SetActive(true);
        return clone;
    }

    public void Despawn(PoolBehaviour pb)
    {
        if (!instances.ContainsKey(pb))
        {
            Debug.LogWarning($"PoolManager ] 오브젝트 풀에 {pb.name} 없음");
            return;
        }

        instances[pb].Release(pb);
        instances.Remove(pb);
    }
}

