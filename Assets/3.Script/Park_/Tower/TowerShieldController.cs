using DG.Tweening;
using Mirror;
using UnityEngine;

public class TowerShieldController : NetworkBehaviour
{
    [SerializeField] private float rotationSpeed = 72f; // 초당 72도 (360도 / 5초)

    void Update()
    {
        // Y축을 기준으로 계속 회전
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}