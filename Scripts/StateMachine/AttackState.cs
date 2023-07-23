using Godot;

namespace Horror.Scripts.StateMachine;

public class AttackState : State
{
    public override void Enter()
    {
    }

    public override void Exit()
    {
    }

    public override void Update(double delta)
    {
        GD.Print("Attack update");
    }

    public override void PhysicsUpdate(double delta)
    {
    }
}