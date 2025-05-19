using System.Collections.Generic;
using JetBrains.Annotations;
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
        EffectFactory.CreateEffects(player, player.effectTypes);
    }

    public override void Perform()
    {
        Debug.Log($"player Attack! : Damage [{player.attackDamage}]");

        if (player.target == null)
        {
            Debug.Log("공격 타겟이 없음");
            return;
        }
        // player와 대상간의 거리 측정
        if (Vector3.Distance(player.transform.position, player.target.transform.position) > player.attackableRange)
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

public class PlayerDetection : PlayerAbility
{
    public PlayerDetection(PlayerController player) : base(player) { }

    public override void Perform()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // 클릭해서 레이를 쏘고 레이에 맞은 Enemy 태그의 플레이어를 player.target으로 둔다.
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Attackable")))
        {
            if (hit.transform.TryGetComponent(out PlayerController enemy) && enemy.teamData.IsEnemy(player.teamData))
            {
                player.target = enemy;
            }
            else
            {
                player.target = null;
            }
        }
        else
        {
            player.target = null;
        }
    }
}