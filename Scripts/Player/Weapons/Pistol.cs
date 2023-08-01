using Godot;

namespace Horror.Scripts.Player.Weapons;

public partial class Pistol : FirearmBase
{
	private Label3D _ammoLabel;

	public override void _Ready()
	{
		base._Ready();
		
		// _ammoLabel = GetNode<Label3D>("pistol1/AmmoIndicator");
		// _ammoLabel.Text = CurrentAmmo.ToString();
	}

	public override void TakeOut()
	{
		Animator.Play("show");
	}

	public override void PutAway()
	{
		Animator.Play("put_away");
	}

	public override bool CanShoot()
	{
		return true;
	}
}