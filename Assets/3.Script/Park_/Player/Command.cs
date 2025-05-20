using UnityEngine;

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
    PlayerAttack attack;
    public AttackCommand(PlayerController player, PlayerAttack attack)
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

    public DetectionCommand(PlayerController player, PlayerDetection detection)
    {
        this.player = player;
        this.detection = detection;
    }

    public void Execute()
    {
        detection.Perform();
    }
}