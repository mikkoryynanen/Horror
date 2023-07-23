using Godot;

public abstract class State
{
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update(double delta);
    public abstract void PhysicsUpdate(double delta);
}
