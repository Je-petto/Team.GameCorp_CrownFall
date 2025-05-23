using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] particleSystems;
    public float extraWaitTime = 0.5f; // 여유 시간 (안정적인 disable을 위해)

    private void Start()
    {
        StartCoroutine(PlayParticlesSequentially_Co());
    }

    private IEnumerator PlayParticlesSequentially_Co()
    {
        foreach (var ps in particleSystems)
        {
            ps.gameObject.SetActive(true);
            ps.Play();

            // 파티클이 완전히 끝날 때까지 대기
            yield return new WaitUntil(() => !ps.IsAlive(true));
            
            // 잠깐 대기 후 파티클 비활성화
            yield return new WaitForSeconds(extraWaitTime);
            ps.gameObject.SetActive(false);
        }
    }
}