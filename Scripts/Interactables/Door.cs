using Godot;

namespace Horror.Scripts.Interactables;

public partial class Door : AnimatableBody3D, IUnlockable
{
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void Unlock()
	{
		_animationPlayer.Play("open");
	}
}