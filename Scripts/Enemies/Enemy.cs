using Godot;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Enemies;

public partial class Enemy : Node3D
{
	[Export()] private float _attackDistance = 1f;
	[Export()] private float _attackSpeed = 0.5f;
	
	private Node3D _target;
	private NavigationAgent3D _navAgent;
	private Area3D _awarenessArea;
	
	private float _attackTimer = 0f;

	private enum State { None, Idle, Chase, Attack }

	private State _currentState = State.None;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent");
		_awarenessArea = GetNode<Area3D>("AwarenessArea");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_awarenessArea.BodyEntered += OnPlayerSeen;
	
		_navAgent.PathDesiredDistance = 0.25f;
		
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState == State.Chase)
		{
			if (_navAgent.IsTargetReachable() && !_navAgent.IsTargetReached())
			{
				var nextLocation = _navAgent.GetNextPathPosition();
				var direction = GlobalPosition.DirectionTo(nextLocation);
				GlobalPosition += direction * (float)delta;
				LookAtFromPosition(this.GlobalPosition, _target.GlobalPosition, Vector3.Up, true);
			}	
		}
		else if (_currentState == State.Attack)
		{
			AttackRoutine((float)delta);
		}

		AiProcess();
	}

	/// <summary>
	/// Processes the AI related logic every 20th frame
	/// </summary>
	private void AiProcess()
	{
		if (Engine.GetFramesDrawn() % 20 == 0)
		{
			switch (_currentState)
			{
				case State.None:
					// GD.Print("Enemy state is none");
					break;
				case State.Idle:
					GD.Print("Enemy idling...");
					if (!_animationPlayer.IsPlaying())
						_animationPlayer.Play("idle");
					break;
				case State.Attack:
					GD.Print($"Attacking {_target.Name}");
					
					if (GetTargetDistance() > _attackDistance)
					{
						_currentState = State.Chase;
					}
					break;
				case State.Chase:
					GD.Print($"Chasing {_target.Name}");
					_navAgent.TargetPosition = _target.GlobalPosition;
					
					_animationPlayer.Play("run/Run");
					
					if (GetTargetDistance() < _attackDistance)
					{
						_currentState = State.Attack;
					}
					break;
			}
		}
	}

	private void AttackRoutine(float delta)
	{
		_attackTimer += delta;
		if (_attackTimer >= _attackSpeed)
		{
			_animationPlayer.Play("punch/Punch");
			
			if (_target is IDamageable damageable)
			{
				damageable.TakeDamage(10);
			}
			_attackTimer = 0f;
		}
	}

	private float GetTargetDistance()
	{
		var distance = this.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition);
		return distance;
	}

	private void OnPlayerSeen(Node3D body)
	{
		GD.Print($"i saw {body.Name}");

		_target = body;
		_navAgent.TargetPosition = body.GlobalPosition;
		_currentState = State.Chase;
	}
}