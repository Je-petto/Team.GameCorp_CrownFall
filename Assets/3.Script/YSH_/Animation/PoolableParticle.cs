using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticle : PoolBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        if (!TryGetComponent(out ps))
            Debug.LogWarning("PoolableParticle ] ParticleSystem 없음");
    }

    void OnEnable()
    {
        if (ps != null)
            ps.Play();
    }

    void OnDisable()
    {
        Despawn(); // PoolBehaviour에서 상속받은 풀 반환 메서드
    }
}

