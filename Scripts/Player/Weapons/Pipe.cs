using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Player.Weapons;

// TODO Move
public interface IWeapon
{
	void Shoot();
	void PlayHitAudio(bool hitDamageable);
	void TakeOut();
	void PutAway();
	bool CanShoot();
}

public partial class Pipe : Node3D, IWeapon
{
	private AnimationPlayer _animator;

	public override void _Ready()
	{
		_animator = GetNode<AnimationPlayer>("AnimationPlayer");
		_animator.AnimationFinished += name => _animator.Play("idle");
	}

	public void Shoot()
	{
		if (!CanShoot()) return;
		
		_animator.Play("melee");
		
	}

	public void PlayHitAudio(bool hitDamageable)
	{
		GetNode<GodotObject>("/root/Root/AudioPlayer").Call(  !hitDamageable ? "on_melee" : "on_melee_hit");
		// AudioManager.Instance.PlayClip(stream);
	}

	public void TakeOut()
	{
		_animator.Play("show");
	}

	public void PutAway()
	{
		_animator.Play("put_away");
	}

	public bool CanShoot()
	{
		return true;
		// return _animator.CurrentAnimation == "idle" && AudioManager.Instance.HasPlayedClip();
	}
}