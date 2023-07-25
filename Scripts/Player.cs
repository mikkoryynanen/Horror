using Godot;
using System;
using Horror.Scripts;
using Horror.Scripts.Autoload;
using Horror.Scripts.Damage;
using Horror.Scripts.Objects;

public partial class Player : CharacterBody3D, IDamageable
{
	[Export()] private float _sensitivity = 0.2f;
	
	private float _minAngle = -80;
	private float _maxAngle = 90;
	private Vector3 _lookRotation;
	private Node3D _head;
	private RayCast3D _raycast;
	
	[Export()] private float Speed = 5.0f;
	[Export()] private float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	// weapon sway
	[Export()] private float swaySpeed = 8f;
	[Export()]	private float amplitude = 0.5f;
	private Vector3 _originalHeadPosition;
	private float _accumulativeDelta;
	private Camera3D _camera;

	private AudioStreamPlayer _footstepsAudio;
	private WeaponManager _weaponManager;
	
	private float _footstepTimer;
	[Export()] private float _footstepInterval = 2;
	private bool _canProcess = true;
	private Area3D _meleeWeaponHitbox;
	private int _health = 100;
	private Horror.Scripts.Player.ViewObjectManager _viewObjectManager;


	public override void _Ready()
	{
		_head = GetNode<Node3D>("Head");
		_camera = GetNode<Camera3D>("Head/Camera3D");
		_raycast = GetNode<RayCast3D>("Head/Camera3D/Hitscan");
		_footstepsAudio = GetNode<AudioStreamPlayer>("Footstep");
		// _weaponManager = GetNode<WeaponManager>("Head/Camera3D/WeaponContainer");
		
		//TODO Move
		_viewObjectManager = GetNode<Horror.Scripts.Player.ViewObjectManager>("Head/Camera3D/ViewObjectCamera");

		// TODO Move
		// _meleeWeaponHitbox = GetNode<Area3D>("Head/Camera3D/WeaponPivot/WeaponMesh/Hitbox");

		_originalHeadPosition = _camera.Position;
		
		this.GetSignalBus().OnStartDialog += (act, title, isFullscren) =>
		{
			if(isFullscren)
				_canProcess = false;
		};
		this.GetSignalBus().OnEndDialog += () => _canProcess = true;
		this.GetSignalBus().OnActivatePlayerCamera += () =>
		{
			_canProcess = true;
			_camera.MakeCurrent();
		};
		this.GetSignalBus().OnDeactivatePlayerCamera += () =>
		{
			_canProcess = false;
			_camera.ClearCurrent();
		};
	}

	public override void _Process(double delta)
	{
		_viewObjectManager.UpdateCameraTransform(_camera.GlobalTransform);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_canProcess) return;
		
		_accumulativeDelta += (float)delta;
		_head.RotationDegrees = new Vector3(_lookRotation.X, _head.RotationDegrees.Y, _head.RotationDegrees.Z);
		this.RotationDegrees = new Vector3(this.RotationDegrees.X, _lookRotation.Y, this.RotationDegrees.Z);
		
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

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
				if (Engine.GetPhysicsFrames() % 2 == 0)
				{
					// this.EmitSignalBus("OnHideInteract");
				}
			}
		}
		
		// TODO Move this to common weapon manager script
		// Melee weapon
		if (Input.IsActionJustPressed("shoot") && _viewObjectManager.equippedWeapon.CanShoot())
		{
			var overlappingBodies = GetNode<Area3D>("Head/Camera3D/MeleeCollisionArea").GetOverlappingBodies();
			var hitDamageable = false;
			foreach (var body in overlappingBodies)
			{
				if (body.IsInGroup("enemy"))
				{
					if (body is IDamageable damageable)
					{
						damageable.TakeDamage(10);
						hitDamageable = true;
						
						// TODO Enable?
						// var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/Particles/HitParticle.tscn");
						// var instance = packed.Instantiate() as Node3D;
						// instance.Position = _raycast.GetCollisionPoint();
						// GetNode<Node3D>("/root/Core").AddChild(instance);
					}
				}
			}
			
			_viewObjectManager.Shoot();
			_viewObjectManager.PlayHitAudio(hitDamageable);
		}

		var inputValue = Mathf.Floor(Mathf.Abs(inputDir.X) + Mathf.Abs(inputDir.Y));
		// Weapon Sway
		var headBob = inputValue * _accumulativeDelta * swaySpeed;
		var targetPos = _originalHeadPosition + Vector3.Up * Mathf.Sin(headBob) * amplitude;
		_camera.Position = _camera.Position.Lerp(targetPos, (float)delta);
		
		// Walking sound
		_footstepTimer += (float)delta;
		if ((!_footstepsAudio.Playing && inputValue > 0) && _footstepTimer >= _footstepInterval)
		{
			_footstepsAudio.PitchScale = (float)GD.RandRange(0.95f, 1.1f);
			_footstepsAudio.Play();
			_footstepTimer = 0;
		}
		else
		{
			_footstepsAudio.Stop();
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (inputEvent is InputEventMouseMotion)
		{
			var motion = inputEvent as InputEventMouseMotion;
			Vector3 lookVector = _lookRotation;
			lookVector.Y -= motion.Relative.X * _sensitivity;
			lookVector.X -= motion.Relative.Y * _sensitivity;
			lookVector.X = Mathf.Clamp(lookVector.X, _minAngle, _maxAngle);
			_lookRotation = lookVector;
			
			_viewObjectManager.Sway(new Vector2(motion.Relative.X, motion.Relative.Y));
		}
	}

	public void TakeDamage(int amount)
	{
		_health -= amount;
		if(_health <= 0)
			GD.Print("GAme over!!!");
	}
}
