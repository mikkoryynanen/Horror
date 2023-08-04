namespace Horror.Scripts.Player.Weapons;

public partial class Unarmed : MeleeBase
{
    public override void _Ready()
    {
        base._Ready();
		
        Animator.AnimationFinished += name =>
        {
            // if (name != "idle")
            //     Animator.Play("idle");
        };
    }

    public override void Shoot()
    {
        base.Shoot();
		
        Animator.Play("ArmsPistolAnimationLibrary/Shoot");
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