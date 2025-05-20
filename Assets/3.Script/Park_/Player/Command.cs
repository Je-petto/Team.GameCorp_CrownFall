public interface ICommand
{
    public void Execute();
}

public class MoveCommand : ICommand
{
    PlayerController player;
    public MoveCommand(PlayerController player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.stateMachine.ChangeState(new MoveState(player));
    }
}

public class IdleCommand : ICommand
{
    PlayerController player;
    public IdleCommand(PlayerController player)
    {
        this.player = player;
    }

    public void Execute()
    {
        player.stateMachine.ChangeState(new IdleState());
    }
}

public class AttackCommand : ICommand
{
    PlayerController player;

    // PlayerAttackNonTargeting attack;

    PlayerAttackIK attack;

    public AttackCommand(PlayerController player, PlayerAttackIK attack)
    {
        this.player = player;
        this.attack = attack;
    }

    public void Execute()
    {
        attack.Perform();
    }
}

public class DetectionCommand : ICommand
{
    PlayerController player;
    PlayerDetection detection;

    bool detectionState;

    public DetectionCommand(PlayerController player, PlayerDetection detection)
    {
        this.player = player;
        this.detection = detection;
        detectionState = false;
    }

    public void OnOff(bool on)
    {
        detectionState = on;
        player.lineRenderer.enabled = on;
    }

    public void Execute()
    {
        if (!detectionState) return;

        detection.Perform();
    }
}