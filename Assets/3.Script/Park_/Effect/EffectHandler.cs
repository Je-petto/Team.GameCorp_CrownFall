using DG.Tweening;
using UnityEngine;

public class EffectHandler
{
    PlayerController player;
    public EffectHandler(PlayerController player)
    {
        this.player = player;
    }

    public void ApplyDamage(int amount)
    {
        Debug.Log($"{amount} -? Damage Apply");
        player.currentStat.hp -= amount * (100 - player.data.defense) / 100;
        player.currentStat.hp = Mathf.Clamp(player.currentStat.hp, 0, player.data.hp);
       
    }

    public void ApplyHeal(float amount)
    {
        Debug.Log("Heal Apply");
        player.currentStat.hp += (int)amount;
        player.currentStat.hp = Mathf.Clamp(0, player.data.hp, player.currentStat.hp);
    }

    public void ApplySlow(float duration, float amount)
    {
        Debug.Log("Slow Apply");
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => player.currentStat.moveSpeed *= (amount / 100f))
            .AppendInterval(duration)
            .OnComplete(() => player.currentStat.moveSpeed = player.data.speed);
    }

    public void ApplyDot(float duration, float amount)
    {
        Debug.Log("Dot Apply");
        int tickCount = Mathf.FloorToInt(duration);
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < tickCount; i++)
        {
            seq.AppendInterval(1f) // 1초 대기
            .AppendCallback(() => ApplyDamage((int)amount)); // 데미지 적용
        }
    }

}