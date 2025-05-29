using DG.Tweening;
using UnityEngine;

public class EffectHandler
{
    PlayerController_Net player;
    public EffectHandler(PlayerController_Net player)
    {
        this.player = player;
    }

    public void ApplyDamage(int amount)
    {
        Debug.Log($"{amount} -? Damage Apply");
        player.hp -= amount * (100 - player.defense) / 100;
        player.hp = Mathf.Clamp(player.hp, 0, player.data.hp);
       
    }

    public void ApplyHeal(float amount)
    {
        Debug.Log("Heal Apply");
        player.hp += (int)amount;
        player.hp = Mathf.Clamp(0, player.data.hp, player.hp);
    }

    public void ApplySlow(float duration, float amount)
    {
        Debug.Log("Slow Apply");
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => player.speed *= (amount / 100f))
            .AppendInterval(duration)
            .OnComplete(() => player.speed = player.data.speed);
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