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
        player.currentStat.hp -= amount * (100 - player.data.defense) / 100;
        player.currentStat.hp = Mathf.Clamp(0, player.data.hp, player.currentStat.hp);
        player.RaiseOnChangeHp();
    }

    public void ApplyHeal(float amount)
    {
        player.currentStat.hp += (int)amount;
        player.currentStat.hp = Mathf.Clamp(0, player.data.hp, player.currentStat.hp);
        player.RaiseOnChangeHp();
    }

    public void ApplySlow(float duration, float amount)
    {
        Sequence seq = DOTween.Sequence();

        seq.AppendCallback(() => player.currentStat.moveSpeed *= (amount / 100f))
            .AppendInterval(duration)
            .OnComplete(() => player.currentStat.moveSpeed = player.data.speed);
    }

    public void ApplyDot(float duration, float amount)
    {
        int tickCount = Mathf.FloorToInt(duration); // 1초마다 실행, 총 횟수
        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < tickCount; i++)
        {
            seq.AppendInterval(1f) // 1초 대기
            .AppendCallback(() => ApplyDamage((int)amount)); // 데미지 적용
            player.RaiseOnChangeHp();
        }
    }

}