using UnityEngine;

public class PlayerIKController : MonoBehaviour
{
    private Animator animator;
    private PlayerController player;

    void Start()
    {
        TryGetComponent(out animator);
        player = GetComponentInParent<PlayerController>();
    }

    void OnAnimatorIK()
    {
        if (animator == null) return;

        if (player.targetPoint == Vector3.zero)
        {
            Debug.Log("타겟 없음...");
            return;
        }

        Debug.Log("IK 회전!");
        animator.SetLookAtWeight(1.0f, 1.0f, 0.1f, 0.0f, 0.4f);
        animator.SetLookAtPosition(player.targetPoint);
    }
}
