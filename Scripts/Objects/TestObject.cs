using Godot;

namespace Horror.Scripts.Objects;

public partial class TestObject : StaticBody3D, IInteractable
{
    private Node3D _source;

    public void Interact()
    {
        this.EmitSignalBus("OnStartDialog", "act_0_1", "act_0_1", false);
    }
}