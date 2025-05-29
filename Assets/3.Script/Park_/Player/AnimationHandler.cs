using Mirror;
using UnityEngine;

public class AnimationHandler : NetworkBehaviour
{
    PlayerController_Net player;
    void Start()
    {
        if (!isLocalPlayer) return;
        this.player = GetComponentInParent<PlayerController_Net>();
    }

    public void EnterSkillCastEvent()
    {
        if (!isLocalPlayer) return;
        Debug.Log("스킬 캐스팅 Enter..");
        player.inputHandler.isStop = true;
    }
    
    public void ExitSkillCastEvent()
    {
        if (!isLocalPlayer) return;
        Debug.Log("스킬 캐스팅 Exit..");
        player.inputHandler.isStop = false;
    }
}
