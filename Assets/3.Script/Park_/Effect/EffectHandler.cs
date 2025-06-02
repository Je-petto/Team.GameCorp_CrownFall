using DG.Tweening;
using Mirror;
using UnityEngine;

public class EffectHandler : NetworkBehaviour
{
    [SerializeField] PlayerController_Net player;

    public void Init(PlayerController_Net player)
    {
        this.player = player;
    }


    // [ClientRPC]
    // public void SetAnimation()
    // {
        
    // }
}