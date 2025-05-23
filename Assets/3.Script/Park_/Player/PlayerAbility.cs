using System.Collections.Generic;
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

public class PlayerAttack : PlayerAbility
{
    List<IEffect> damageEffect = new();
    public PlayerAttack(PlayerController player) : base(player)
    {
        SkillData attackData = player.data.skillSet.Find(data => data.type.Equals(SkillType.NONE));
        damageEffect = EffectFactory.CreateSkillEffects(attackData);
    }

    public override void Perform()
    {
        Debug.Log($"player Attack! : Damage [{player.data.attack}]");

        if (player.target == null)
        {
            Debug.Log("공격 타겟이 없음");
            return;
        }
        // player와 대상간의 거리 측정
        if (Vector3.Distance(player.transform.position, player.target.transform.position) > player.data.attackableRange)
        {
            Debug.Log("공격 타겟이 너무 멀다.");
            return;  
        } 
        
        Debug.Log($"{player.target} 공격 성공!!");

        foreach (var effect in damageEffect)
        {
            effect.Apply(player.target);
        }
    }
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
    private LineRenderer lineRenderer;
    public PlayerAttackNonTargeting(PlayerController player) : base(player){ }

    public override void Perform()
    {
        Shoot();
    }

    public void Shoot()
    {
        // 추후 오브젝트 풀링으로 변경 예정.

        GameObject projection = GameObject.Instantiate(player.data.projection, player.attackPoint.transform.position, Quaternion.identity);
        player.transform.DOLookAt(player.targetPoint, 0.1f)
        .OnComplete(()=> { projection.transform.DOMove(player.targetPoint, 0.2f).OnComplete(() => { GameObject.Destroy(projection); }); });
    }
}

// non-target [IK]
public class PlayerAttackIK : PlayerAbility
{
    private int attackIndex = 0;

    public PlayerAttackIK(PlayerController player) : base(player) { }

    public override void Perform()
    {
        player.animator.SetFloat("AttackIndex", attackIndex);

        attackIndex++;
        attackIndex = attackIndex % 2;          //0과 1만 존재.

        player.animator.SetTrigger("Attack");

        Shoot();
    }

    public void Shoot()
    {
        // 추후 오브젝트 풀링으로 변경 예정.
        GameObject projection = GameObject.Instantiate(player.data.projection, player.attackPoint.transform.position, Quaternion.identity);

        projection.transform.DOMove(player.targetPoint, 0.2f)
        .OnComplete(() =>
        {
            player.targetPoint = Vector3.zero;
            GameObject.Destroy(projection);
        });

        SequenceUpperBody();
    }

    private void SequenceUpperBody()
    {
        Transform ub = player.animator.GetBoneTransform(player.bone);
        ub.LookAt(player.targetPoint);
    }
}
#endregion  

public class PlayerSkill : PlayerAbility
{
    private SkillData skillData;
    public List<IEffect> effects;           //스킬이 주는 효과.

    public PlayerSkill(PlayerController player) : base(player)
    {
        skillData = player.data.skillSet.Find(data => !data.Equals(SkillType.NONE));
        effects = EffectFactory.CreateSkillEffects(skillData);
    }

    public override void Perform()
    {
        
    }
}