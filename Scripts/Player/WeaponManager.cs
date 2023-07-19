using System.Linq;
using Godot;
using Horror.Scripts;

public partial class WeaponManager : Node3D
{
	// Current weapon settings
	[Export()] private AudioStream audioStream;
	[Export()] private bool isAutomatic = false;
	[Export()] private float fireRate = .5f;
	
	private RayCast3D _raycast;
	private Camera3D _camera;
	private AudioStreamPlayer _audioStreamPlayer;
	private Label3D _ammoLabel;

	private float _pressedDownTimer;
	private bool _isPressingDown;
	private int _currentAmmo = 15;
	private bool _canProcess = true;


	public override void _Ready()
	{
		_raycast = GetNode<RayCast3D>("Hitscan");
		_camera = GetViewport().GetCamera3D();
		
		_ammoLabel = GetNode<Label3D>("pistol1/AmmoIndicator");
		_ammoLabel.Text = _currentAmmo.ToString();
		
		_audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		_audioStreamPlayer.Stream = audioStream;
		
		this.GetSignalBus().OnStartDialog += (act, title, isFullscren) =>
		{
			if(isFullscren)
				_canProcess = false;
		};
		this.GetSignalBus().OnEndDialog += () => _canProcess = true;
	}

	public override void _Process(double delta)
	{
		if (!_canProcess) return;
		
		var screenPoint = new Vector2I((int)GetViewport().GetVisibleRect().Size.X / 2, (int)GetViewport().GetVisibleRect().Size.Y / 2);
		var from = _camera.ProjectRayOrigin(screenPoint);
		var to = from + _camera.ProjectRayNormal(screenPoint) * 100;
		
		var spaceState = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to);
		var result = spaceState.IntersectRay(query);
		if (result.Count > 0)
		{
			// GD.Print($"hit {result["collider"]}");
		}
		
		if (_currentAmmo > 0 && Input.IsActionJustPressed("shoot"))
		{
			_isPressingDown = true;
			if (!isAutomatic)
			{
				FireProjectile();
			}
		}

		if (Input.IsActionJustReleased("shoot"))
		{
			_pressedDownTimer = 0;
			_isPressingDown = false;
		}

		if (isAutomatic && _isPressingDown && _currentAmmo > 0)
		{
			_pressedDownTimer += (float)delta;
			if (_pressedDownTimer >= fireRate)
			{
				FireProjectile();
				_pressedDownTimer = 0;
				
				_currentAmmo--;
				_ammoLabel.Text = _currentAmmo.ToString();
			}
		}
	}

	private void FireProjectile()
	{
		var packed = ResourceLoader.Load("res://Prefabs/player/flash.tscn") as PackedScene;
		var instance = packed.Instantiate() as Node3D;
		instance.GetNode<AnimationPlayer>("AnimationPlayer").Play("flash");
		GetNode("pistol1/Muzzle").AddChild(instance);

		_audioStreamPlayer.Play();
	}
}
