using Godot;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Objects;

public partial class TestObject : StaticBody3D, IInteractable, IDamageable
{
    private int _health = 20;


    public override void _Ready()
    {
    }

    public void Interact()
    {
        this.EmitSignalBus("OnStartDialog", "act_0_1", "act_0_1", false);
    }

    public void HoldInteract()
    {
        throw new System.NotImplementedException();
    }

    public void TakeDamage(int amount)
    {
        if (_health <= 0) return;
        
        GD.Print($"taking {amount} damage");

        _health -= amount;
        // if(_health <= 0)
        //     QueueFree();
    }
}