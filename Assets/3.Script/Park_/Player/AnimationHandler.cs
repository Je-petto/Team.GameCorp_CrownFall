using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    PlayerController player;
    void Start()
    {
        this.player = GetComponentInParent<PlayerController>();
    }

    public void EnterSkillCastEvent()
    {
        Debug.Log("스킬 캐스팅 Enter..");
        player.inputHandler.isStop = true;
    }
    
    public void ExitSkillCastEvent()
    {
        Debug.Log("스킬 캐스팅 Exit..");
        player.inputHandler.isStop = false;
    }
}
