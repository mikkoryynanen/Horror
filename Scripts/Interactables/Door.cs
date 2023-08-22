using Godot;

namespace Horror.Scripts.Interactables;

public partial class Door : AnimatableBody3D, IInteractable
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void Interact()
	{
		_animationPlayer.Play("open");
	}
}