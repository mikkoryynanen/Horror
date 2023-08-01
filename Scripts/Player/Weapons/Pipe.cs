using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Player.Weapons;

// TODO Move


public partial class Pipe : MeleeBase
{
	private AnimationPlayer _animator;

	public override void _Ready()
	{
		_animator = GetNode<AnimationPlayer>("AnimationPlayer");
		_animator.AnimationFinished += name => _animator.Play("idle");
	}

	public override void Shoot()
	{
		if (!CanShoot()) return;
		
		_animator.Play("melee");
		
	}

	public override void TakeOut()
	{
		_animator.Play("show");
	}

	public override void PutAway()
	{
		_animator.Play("put_away");
	}

	public override bool CanShoot()
	{
		return _animator.CurrentAnimation == "idle";
	}
}