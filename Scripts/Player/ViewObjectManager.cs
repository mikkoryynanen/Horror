using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Player.Weapons;

namespace Horror.Scripts.Player;

public partial class ViewObjectManager : Node
{
	[Export()] private float intensity = 0.0005f;
	
	private Node3D _rig;
	private float _t;
	private Camera3D _objectCamera;
	public IWeapon equippedWeapon;


	public override void _Ready()
	{
		_rig = GetNode<Node3D>("SubViewport/ObjectCamera/Rig");
		_objectCamera = GetNode<Camera3D>("SubViewport/ObjectCamera");
		
		//TODO Requires further logic to be scaleable
		equippedWeapon = GetNode<Pipe>("SubViewport/ObjectCamera/Rig/Pipe");

		this.GetSignalBus().OnActivatePlayerCamera += TakeOut;
		this.GetSignalBus().OnDeactivatePlayerCamera += PutAway;
	}

	public override void _Process(double delta)
	{
		_t = (float)delta * 20f;

		_rig.Position = new Vector3(
			Mathf.Lerp(_rig.Position.X, 0, _t), 
			Mathf.Lerp(_rig.Position.Y, 0, _t),
			Mathf.Lerp(_rig.Position.Z, 0, _t));
	}

	public void UpdateCameraTransform(Transform3D transform)
	{
		_objectCamera.GlobalTransform = transform;
	}

	public void Sway(Vector2 swayAmount)
	{
		_rig.Position += new Vector3(swayAmount.X * -intensity, swayAmount.Y * intensity, 0);
	}

	public void Shoot()
	{
		equippedWeapon.Shoot();
	}
	
	// TODO Move
	public void PlayHitAudio(bool hitDamageable)
	{
		equippedWeapon.PlayHitAudio(hitDamageable);
	}

	public void PutAway()
	{
		equippedWeapon.PutAway();
	}
	
	public void TakeOut()
	{
		equippedWeapon.TakeOut();
	}
}