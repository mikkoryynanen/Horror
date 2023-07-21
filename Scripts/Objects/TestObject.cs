using Godot;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Objects;

public partial class TestObject : StaticBody3D, IInteractable, IDamageable
{
    private Node3D _source;

    private int _health = 20;


    public override void _Ready()
    {
    }

    public void Interact()
    {
        this.EmitSignalBus("OnStartDialog", "act_0_1", "act_0_1", false);
    }

    public void TakeDamage(int amount)
    {
        GD.Print($"taking {amount} damage");

        _health -= amount;
        if(_health <= 0)
            QueueFree();
    }
}