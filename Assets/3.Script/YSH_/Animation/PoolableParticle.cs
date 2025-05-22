using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolableParticle : PoolBehaviour
{
    private ParticleSystem ps;
    private Coroutine despawnRoutine;

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

    public void SetDuration(float duration)
    {
        if (despawnRoutine != null)
            StopCoroutine(despawnRoutine);

        if (duration > 0)
            despawnRoutine = StartCoroutine(DespawnAfterDelay(duration));
    }

    IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Despawn(); // 풀로 반환
    }

    void OnDisable()
    {
        if (despawnRoutine != null)
        {
            StopCoroutine(despawnRoutine);
            despawnRoutine = null;
        }
    }
}
