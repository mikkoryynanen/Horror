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
	// TODO If we want these to sound different based on what we hit, Consider moving these to resource, where we fetch them based on material hit
	[Export()] private AudioStream[] _missAudioStreams;
	[Export()] private AudioStream[] _hitAudioStreams;
	
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
		var stream = hitDamageable ? 
			_hitAudioStreams[GD.RandRange(0, _hitAudioStreams.Length - 1)]: 
			_missAudioStreams[GD.RandRange(0, _missAudioStreams.Length - 1)];
		AudioManager.Instance.PlayClip(stream);
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
		return _animator.CurrentAnimation == "idle" && AudioManager.Instance.HasPlayedClip();
	}
}