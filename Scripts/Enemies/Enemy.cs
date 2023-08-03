using System.Threading.Tasks;
using Godot;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Enemies;

public partial class Enemy : CharacterBody3D, IDamageable
{
	[Export()] private float _attackDistance = 1f;
	[Export()] private float _attackSpeed = 0.5f;
	
	private Node3D _target;
	private NavigationAgent3D _navAgent;
	private Area3D _awarenessArea;
	
	private float _attackTimer = 0f;

	private enum State { None, Idle, Chase, Attack, AttackSwing }

	private State _currentState = State.None;
	private AnimationPlayer _animationPlayer;
	private Area3D _hurtbox;
	
	private const int animTotalLength = 3866;
	private const int animAttackTime = 1100;
	private bool _swingRoutineRunning = false;
	private int _health = 500;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent");
		_awarenessArea = GetNode<Area3D>("AwarenessArea");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_hurtbox = GetNode<Area3D>("Hurtbox");

		_awarenessArea.BodyEntered += OnPlayerSeen;
		
		_navAgent.PathDesiredDistance = 0.25f;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_navAgent.IsTargetReachable() && !_navAgent.IsTargetReached())
		{
			if (_currentState == State.Chase)
			{
				Move((float)delta);
			}

		}
		if ( _currentState == State.Attack)
		{
			Move((float)delta);
		}
		AiProcess((float)delta);
	}
	
	public void TakeDamage(int amount)
	{
		_health -= amount;
		if(_health <= 0)
			QueueFree();
	}

	private void Move(float delta)
	{
		var nextLocation = _navAgent.GetNextPathPosition();
		var direction = GlobalPosition.DirectionTo(nextLocation);
		GlobalPosition += direction * delta;
		LookAtFromPosition(this.GlobalPosition, _target.GlobalPosition, Vector3.Up, true);
	}

	/// <summary>
	/// Processes the AI related logic every 20th frame
	/// </summary>
	private void AiProcess(float delta)
	{
		if (Engine.GetFramesDrawn() % 20 == 0)
		{
			switch (_currentState)
			{
				case State.None:
					// GD.Print("Enemy state is none");
					break;
				case State.Idle:
					// GD.Print("Enemy idling...");
					if (!_animationPlayer.IsPlaying())
						_animationPlayer.Play("idle");
					break;
				case State.Attack:
					// GD.Print($"Attacking {_target.Name}");
					
					_navAgent.TargetPosition = _target.GlobalPosition;
			
					if(!_swingRoutineRunning)
						SwingRoutine(delta);
					
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

	private async Task SwingRoutine(float delta)
	{
		_swingRoutineRunning = true;
		_animationPlayer.Play("punch/Punch");

		_currentState = State.AttackSwing;
		
		await Task.Delay(animAttackTime);
			
		OnCheckMeleeHurtbox();
		
		await Task.Delay(animTotalLength - animAttackTime);

		_currentState = !IsFacingTarget() ? State.Chase : State.Attack;
		_swingRoutineRunning = false;
	}

	private float GetTargetDistance()
	{
		var distance = this.GlobalPosition.DistanceSquaredTo(_target.GlobalPosition);
		return distance;
	}
	
	private void OnCheckMeleeHurtbox()
	{
		var bodies = _hurtbox.GetOverlappingBodies();
		foreach (var body in bodies)
		{
			if (body.IsInGroup("player"))
			{
				if (body is IDamageable damageable)
				{
					damageable.TakeDamage(10);
				}
			}
		}
	}

	private void OnPlayerSeen(Node3D body)
	{
		GD.Print($"i saw {body.Name}");

		_target = body;
		_navAgent.TargetPosition = body.GlobalPosition;
		_currentState = State.Chase;
	}

	private bool IsFacingTarget()
	{
		var forwardVector = GlobalTransform.Basis.Z.Normalized();
		var targetForwardVector = _target.GlobalTransform.Basis.Z.Normalized();

		var dot = forwardVector.Dot(targetForwardVector);
		return dot > 0.25f;
	}
}