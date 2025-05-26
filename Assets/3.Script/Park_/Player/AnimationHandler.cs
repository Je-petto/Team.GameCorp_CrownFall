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
        player.inputHandler.isStop = true;
    }
    
    public void ExitSkillCastEvent()
    {
        player.inputHandler.isStop = false;
    }
}
