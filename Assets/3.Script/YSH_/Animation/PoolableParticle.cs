using System.Collections;
using UnityEngine;

public class PoolableParticle : PoolBehaviour
{
    private ParticleSystem ps;
    private Coroutine returnCo;

    void Awake()
    {
        if (!TryGetComponent(out ps))
            Debug.LogWarning("PoolableParticle ] ParticleSystem 없음");
    }

    void OnEnable()
    {
        if (ps != null)
        {
            ps.Play();

            if (returnCo != null)
                StopCoroutine(returnCo);

            var main = ps.main;
            float duration = main.duration;

            // stopAction은 main에서 접근해야 함
            if (!main.loop && main.stopAction == ParticleSystemStopAction.None)
                returnCo = StartCoroutine(ReturnToPool_Co(duration));
        }
    }

    void OnDisable()
    {
        if (returnCo != null)
        {
            StopCoroutine(returnCo);
            returnCo = null;
        }
    }

    private IEnumerator ReturnToPool_Co(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (gameObject != null && gameObject.activeInHierarchy)
        {
            Despawn();
        }
    }

    public void SetDuration(float duration)
    {
        StartCoroutine(_CoAutoDespawn(duration));
    }

    private IEnumerator _CoAutoDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (PoolManager.I != null)
            PoolManager.I.Despawn(this);
    }
}
