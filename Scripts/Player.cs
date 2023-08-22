using Godot;
using Horror.Scripts;
using Horror.Scripts.Autoload;
using Horror.Scripts.Damage;
using Horror.Scripts.Inventory;
using Horror.Scripts.Player;
using Horror.Scripts.UI;

public partial class Player : CharacterBody3D, IDamageable
{
	// [Export()] private GUIManager GUI; 
	
	[Export()] private float _sensitivity = 0.2f;
	
	private Node3D _head;
	private RayCast3D _raycast;
	
	[Export()] private float WalkingSpeed = 5.0f;
	[Export()] private float RunningSpeed = 5.0f;
	[Export()] private float _staminaDepletionSpeed = 1.5f;
	[Export()] private float _staminaRechargeSpeed = 1.5f;

	// weapon sway
	[Export()] private float swaySpeed = 8f;
	[Export()] private float runningSwaySpeed = 8f;
	[Export()]	private float amplitude = 0.5f;
	[Export()]	private float runningAmplitude = 0.5f;
	private Vector3 _originalHeadPosition;
	private float _accumulativeDelta;
	private Camera3D _camera;

	private Horror.Scripts.Player.WeaponManager _weaponManager;
	
	[Export()] private float _footstepInterval = 2;
	[Export()] private float _footstepIntervalRunning = 1;
	private float _footstepTimer;
	
	private bool _canProcess = true;
	private Area3D _meleeWeaponHitbox;
	private int _health = 100;
	private Horror.Scripts.Player.ViewObjectManager _viewObjectManager;
	private bool _isRunning = false;
	
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public float Stamina { get; private set; } = 1f;


	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		_camera = GetNode<Camera3D>("Head/Camera3D");
		_raycast = GetNode<RayCast3D>("Head/Camera3D/Hitscan");
		
		_weaponManager = GetNode<WeaponManager>("Head/Camera3D/ViewObjectCamera/SubViewport/Root/WeaponContainer");
		_viewObjectManager = GetNode<Horror.Scripts.Player.ViewObjectManager>("Head/Camera3D/ViewObjectCamera");

		// TODO Move
		// _meleeWeaponHitbox = GetNode<Area3D>("Head/Camera3D/WeaponPivot/WeaponMesh/Hitbox");

		_originalHeadPosition = _camera.Position;
		
		var signalBus = this.GetSignalBus();
		signalBus.OnStartDialog += (act, title, isFullscren) =>
		{
			if(isFullscren)
				_canProcess = false;
		};
		signalBus.OnEndDialog += () => _canProcess = true;
		signalBus.OnActivatePlayerCamera += () =>
		{
			_canProcess = true;
			_camera.MakeCurrent();
		};
		signalBus.OnDeactivatePlayerCamera += () =>
		{
			_canProcess = false;
			_camera.ClearCurrent();
		};
		signalBus.OnOpenInventory += () => _canProcess = false;
		signalBus.OnCloseInventory += () => _canProcess = true;
		signalBus.OnReduceStamina += OnReduceStamina;
		
		this.EmitSignalBus(nameof(SignalBus.OnUpdateStamina), Stamina);
		this.EmitSignalBus(nameof(SignalBus.OnUpdateHealth), _health);
	}

	public override void _Process(double delta)
	{
		var lookVector = InputManager.Instance.GetLookVector();
		_viewObjectManager.UpdateView(_camera.GlobalTransform);
		// _weaponManager.Sway(new Vector2(lookVector.X, lookVector.Y));
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_canProcess) return;
		
		_accumulativeDelta += (float)delta;

		var lookVector = InputManager.Instance.GetLookVector();
		
		_head.RotationDegrees = new Vector3(lookVector.X, _head.RotationDegrees.Y, _head.RotationDegrees.Z);
		this.RotationDegrees = new Vector3(this.RotationDegrees.X, lookVector.Y, this.RotationDegrees.Z);
		
		Vector3 velocity = Velocity;

		// Add the gravity.
		// if (!IsOnFloor())
		// 	velocity.Y -= gravity * (float)delta;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		var inputDir = InputManager.Instance.GetMoveVector();
		var direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			var speed = (_isRunning ? RunningSpeed : WalkingSpeed);
			velocity.X = direction.X * speed;
			velocity.Z = direction.Z * speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _isRunning ? RunningSpeed : WalkingSpeed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _isRunning ? RunningSpeed : WalkingSpeed);
		}
		
		// Run
		if (Stamina > 0)
		{
			if (Input.IsActionJustPressed("run"))
			{
				_isRunning = true;
				AudioManager.Instance.PlayClip(AudioManager.AudioClipName.Breathe);
				
				this.EmitSignalBus(nameof(SignalBus.OnCameraShakeContinuous), .35f);
			}
			else if (Input.IsActionJustReleased("run"))
			{
				_isRunning = false;
				AudioManager.Instance.SetBreatheEnd(Stamina > 0.25f ? "Low" : "High");
				this.EmitSignalBus(nameof(SignalBus.OnCameraShakeStop));
			}
		}
		else
			_isRunning = false;

		UpdateStamina(delta);

		Velocity = velocity;
		MoveAndSlide();

		// Interact
		if (_raycast.IsColliding())
		{
			var target = _raycast.GetCollider();
			if (target is IInteractable interactable)
			{
				this.EmitSignalBus("OnShowInteract");
				if (Input.IsActionJustPressed("interact"))
				{
					interactable.Interact();
				}
			}
			else
			{
				// TODO This gets called too often
				this.EmitSignalBus("OnHideInteract");
			}
		}
		
		// Inventory
		if (Input.IsActionJustPressed("inventory"))
		{
			this.EmitSignalBus("OnOpenInventory");
		}

		var inputValue = Mathf.Floor(Mathf.Abs(inputDir.X) + Mathf.Abs(inputDir.Y));
		
		_weaponManager.SetWalkAnimation(inputValue, _isRunning);
		
		// Weapon Sway
		var headBob = inputValue * _accumulativeDelta * (_isRunning ? runningSwaySpeed : swaySpeed);
		var targetPos = _originalHeadPosition + Vector3.Up * Mathf.Sin(headBob) * (_isRunning ? runningAmplitude : amplitude);
		_camera.Position = _camera.Position.Lerp(targetPos, (float)delta);
		
		// Walking sound
		_footstepTimer += (float)delta;
		if (inputValue > 0 && _footstepTimer >= (_isRunning ? _footstepIntervalRunning : _footstepInterval))
		{
			AudioManager.Instance.PlayClip(AudioManager.AudioClipName.Footsteps);
			_footstepTimer = 0;
		}
	}

	private void UpdateStamina(double delta)
	{
		if (_isRunning)
		{
			Stamina -= ((float)delta / 100) * _staminaDepletionSpeed;
			this.EmitSignalBus("OnUpdateStamina", Stamina);
		}
		else
		{
			if (Stamina < 1f)
			{
				Stamina += ((float)delta / 100) * _staminaRechargeSpeed;
				this.EmitSignalBus("OnUpdateStamina", Stamina);
			}	
		}
	}
	
	private void OnReduceStamina(float value)
	{
		Stamina -= value;
		Stamina = Mathf.Max(Stamina, 0);
	}

	public void TakeDamage(int amount)
	{
		_health -= amount;
		GD.Print($"Player taking damage. {_health} / 100");
		if(_health <= 0)
			GD.Print("GAme over!!!");
		
		this.EmitSignalBus(nameof(SignalBus.OnUpdateHealth), _health);
	}
}
