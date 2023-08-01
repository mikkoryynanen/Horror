using Godot;

namespace Horror.Scripts.Player;

public partial class WeaponManager : Node3D
{
	[Export()] private float swayIntensity = 0.0005f;
	
	// Current weapon settings
	[Export()] private bool isAutomatic = false;
	[Export()] private float fireRate = .5f;
	
	private RayCast3D _raycast;
	private Camera3D _camera;
	// private Label3D _ammoLabel;

	private float _pressedDownTimer;
	private bool _isPressingDown;
	private int _currentAmmo = 15;
	private bool _canProcess = true;
	private float _t;

	// public IWeapon CurrentWeapon { get; private set; }


	public override void _Ready()
	{
		// CurrentWeapon = GetNode<IWeapon>("Rig");
		
		// _rig = GetNode<Node3D>("Rig");
		_raycast = GetNode<RayCast3D>("Hitscan");
		_camera = GetViewport().GetCamera3D();
		
		// _ammoLabel = GetNode<Label3D>("pistol1/AmmoIndicator");
		// _ammoLabel.Text = _currentAmmo.ToString();
		
		// _audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		// _audioStreamPlayer.Stream = audioStream;
		
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
		
		// TODO Hitscan logic
		// var screenPoint = new Vector2I((int)GetViewport().GetVisibleRect().Size.X / 2, (int)GetViewport().GetVisibleRect().Size.Y / 2);
		// var from = _camera.ProjectRayOrigin(screenPoint);
		// var to = from + _camera.ProjectRayNormal(screenPoint) * 100;
		//
		// var spaceState = GetWorld3D().DirectSpaceState;
		// var query = PhysicsRayQueryParameters3D.Create(from, to);
		// var result = spaceState.IntersectRay(query);
		// if (result.Count > 0)
		// {
		// 	// GD.Print($"hit {result["collider"]}");
		// }
		
		// if (/*_currentAmmo > 0 && */Input.IsActionJustPressed("shoot"))
		// {
		// 	CurrentWeapon.Shoot();
		// 	// _isPressingDown = true;
		// 	// if (!isAutomatic)
		// 	// {
		// 	// 	FireProjectile();
		// 	// }
		// }
		//
		// if (Input.IsActionJustReleased("shoot"))
		// {
		// 	_pressedDownTimer = 0;
		// 	_isPressingDown = false;
		// }

		// if (isAutomatic && _isPressingDown && _currentAmmo > 0)
		// {
		// 	_pressedDownTimer += (float)delta;
		// 	if (_pressedDownTimer >= fireRate)
		// 	{
		// 		FireProjectile();
		// 		_pressedDownTimer = 0;
		// 		
		// 		_currentAmmo--;
		// 		// _ammoLabel.Text = _currentAmmo.ToString();
		// 	}
		// }
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