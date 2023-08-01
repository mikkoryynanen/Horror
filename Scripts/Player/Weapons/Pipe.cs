namespace Horror.Scripts.Player.Weapons;

public partial class Pipe : MeleeBase
{
	public override void _Ready()
	{
		base._Ready();
		
		Animator.AnimationFinished += name => Animator.Play("idle");
	}

	public override void Shoot()
	{
		base.Shoot();
		
		Animator.Play("melee");
	}

	public override void TakeOut()
	{
		Animator.Play("show");
	}

	public override void PutAway()
	{
		Animator.Play("put_away");
	}
}