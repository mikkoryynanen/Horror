using Godot;

namespace Horror.Scripts.Objects;

public partial class TestObject : StaticBody3D, IInteractable
{
    private Node3D _source;
    
    
    public override void _PhysicsProcess(double delta)
    {
        // if (_source != null)
        // {
        //     _source.Position.Lerp(Position, (float)delta);
        // }
    }

    public void Interact()
    {
        // var monitor = GetNode<Mesh>("Monitor");
        // _source = GetNode<Node3D>("/root/Core/Player");
    }
}