using Godot;

namespace Horror.Scripts.StateMachine;

public class IdleState : State
{
    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        GD.Print("Idle update");
    }

    public override void PhysicsUpdate(double delta)
    {
    }
}