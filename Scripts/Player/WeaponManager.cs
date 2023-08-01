using Godot;

namespace Horror.Scripts.Player;

public partial class WeaponManager : Node3D
{
	[Export()] private float swayIntensity = 0.0005f;

	private float _pressedDownTimer;
	private bool _isPressingDown;
	private bool _canProcess = true;
	private float _t;
	private Area3D _meleeCollisionArea;


	public override void _Ready()
	{
		// _meleeCollisionArea = GetNode<Area3D>("SubViewport/ObjectCamera/WeaponContainer/Rig/MeleeCollisionArea");
		// _meleeCollisionArea.Visible = false;
		
		this.GetSignalBus().OnStartDialog += (act, title, isFullscreen) =>
		{
			if(isFullscreen)
				_canProcess = false;
		};
		this.GetSignalBus().OnEndDialog += () => _canProcess = true;
	}

	public override void _Process(double delta)
	{
		if (!_canProcess) return;
		
		_t = (float)delta * 20f;

		Position = new Vector3(
			Mathf.Lerp(Position.X, 0, _t), 
			Mathf.Lerp(Position.Y, 0, _t),
			Mathf.Lerp(Position.Z, 0, _t));
	}
	
	public void Sway(Vector2 swayAmount)
	{
		Position += new Vector3(swayAmount.X * -swayIntensity, swayAmount.Y * swayIntensity, 0);
	}

	private void FireProjectile()
	{
		var packed = ResourceLoader.Load("res://Prefabs/player/flash.tscn") as PackedScene;
		var instance = packed.Instantiate() as Node3D;
		instance.GetNode<AnimationPlayer>("AnimationPlayer").Play("flash");
		GetNode("pistol1/Muzzle").AddChild(instance);
	}
}