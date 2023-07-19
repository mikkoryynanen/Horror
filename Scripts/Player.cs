using Godot;
using System;
using Horror.Scripts;
using Horror.Scripts.Autoload;

public partial class Player : CharacterBody3D
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

	// Footsteps
	private AudioStreamPlayer _footstepsAudio;
	private float _footstepTimer;
	[Export()] private float _footstepInterval = 2;


	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		_head = GetNode<Node3D>("Head");
		_camera = GetNode<Camera3D>("Head/Camera3D");
		_raycast = GetNode<RayCast3D>("Head/Camera3D/Hitscan");
		_footstepsAudio = GetNode<AudioStreamPlayer>("Footstep");

		_originalHeadPosition = _camera.Position;
	}

	public override void _PhysicsProcess(double delta)
	{
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

		if (_raycast.IsColliding())
		{
			var target = _raycast.GetCollider();
			if (target is IInteractable interactable)
			{
				this.EmitSignalBus(nameof(SignalBus.OnShowInteract));
				if (Input.IsActionJustPressed("interact"))
				{
					interactable.Interact();
				}
			}
			else
			{
				this.EmitSignalBus(nameof(SignalBus.OnHideInteract));
			}
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
		}
	}
}
