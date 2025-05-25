using DG.Tweening;
using UnityEngine;

/*
    ===================
    Player Ability Part
    ===================
*/

public abstract class PlayerAbility
{
    protected PlayerController player;

    public PlayerAbility(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Perform() { }
}

//논 타겟팅 인식.
public class PlayerDetection : PlayerAbility
{
    public PlayerDetection(PlayerController player) : base(player) { }

    public override void Perform()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Ground")))                  //그라운드만 인식.
        {
            player.lineRenderer.SetPosition(0, player.attackPoint.position);

            Vector3 targetPoint = new Vector3(hit.point.x, player.attackPoint.position.y, hit.point.z);

            Vector3 lineDir = (targetPoint - player.attackPoint.position).normalized;

            player.lineRenderer.SetPosition(1, player.attackPoint.position + (lineDir * player.data.attackableRange));
            player.targetPoint = player.attackPoint.position + (lineDir * player.data.attackableRange);
        }
    }
}

#region Attack - Testcase
// non-target [IK]
public class PlayerAttackNonTargeting : PlayerAbility
{
    public PlayerAttackNonTargeting(PlayerController player) : base(player){ canAttack = true; }
    private bool canAttack;
    public override void Perform()
    {
        if (!canAttack) return;
        Shoot();
    }

    public void Shoot()
    {
        canAttack = false;

        GameObject projection = GameObject.Instantiate(
            player.data.projection,
            player.attackPoint.transform.position,
            Quaternion.identity
        );

        player.transform.DOLookAt(player.targetPoint, 0.1f)
            .OnComplete(() =>
            {
                projection.transform.DOMove(player.targetPoint, 0.2f)
                    .OnComplete(() => GameObject.Destroy(projection));
            });

        DOVirtual.DelayedCall(1f, () =>
        {
            canAttack = true;
            Debug.Log("공격 쿨타임 해제됨");
        });
    }
}

#endregion