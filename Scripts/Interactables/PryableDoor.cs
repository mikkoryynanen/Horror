using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Interactables;

public partial class PryableDoor : AnimatableBody3D, IInteractable
{
	[Export()] private float duration = 100f;

	private float _currentDuration = 0f;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_currentDuration = duration;
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void Unlock()
	{
		_animationPlayer.Play("open");
	}

	public void Interact()
	{
	}

	public void HoldInteract()
	{
		if (_currentDuration > 0)
		{
			_currentDuration -= 1f;
			GD.Print($"Interacting... {_currentDuration}");
			this.EmitSignalBus(nameof(SignalBus.OnPry));
		}
		else
		{
			GD.PrintErr("door opened");
			Unlock();
		}
	}
}