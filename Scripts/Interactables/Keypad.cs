using Godot;

namespace Horror.Scripts.Interactables;

public partial class Keypad : StaticBody3D, IInteractable
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Interact()
	{
		GD.PrintErr("NYI Add interact scene here");
		GetParent<IUnlockable>().Unlock();
	}

	public void HoldInteract()
	{
	}
}