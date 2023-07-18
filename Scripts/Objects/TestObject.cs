using Godot;

namespace Horror.Scripts.Objects;

public partial class TestObject : StaticBody3D, IInteractable
{
    public void Interact()
    {
        GD.Print("you clicked on test object");
    }
}