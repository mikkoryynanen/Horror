using System;
using System.Threading.Tasks;
using Godot;
using Godot.Collections;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Enemies;

public partial class Enemy : CharacterBody3D, IDamageable
{
	[Export()] private float _attackDistance = 1f;
	[Export()] private float _attackSpeed = 0.5f;
	[Export()] private float _speed = 1.1f;
	
	private NavigationAgent3D _navAgent;
	private Area3D _awarenessArea;
	
	private float _attackTimer = 0f;

	private enum State { None, Idle, Patrol, Chase, Attack, AttackSwing }

	private State _currentState = State.Patrol;
	private AnimationTree _animationTree;
	private Area3D _hurtbox;
	
	private const int animTotalLength = 3866;
	private const int animAttackTime = 1100;
	private bool _swingRoutineRunning = false;
	private int _health = 100;
	
	private int _currentPathIndex = 0;
	private Array<Node> _patrolNodes;
	private Node3D _target;
	private int _attackTriesCount = 0;

	public override void _Ready()
	{
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent");
		_awarenessArea = GetNode<Area3D>("AwarenessArea");
		_animationTree = GetNode<AnimationTree>("AnimationTree");
		_hurtbox = GetNode<Area3D>("Hurtbox");
			
		_patrolNodes = GetTree().GetNodesInGroup("PatrolNode");

		_awarenessArea.BodyEntered += OnPlayerSeen;
		_awarenessArea.BodyExited += OnPlayerLost;
		
		_navAgent.PathDesiredDistance = 0.25f;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_navAgent.IsTargetReachable() && !_navAgent.IsTargetReached())
		{
			if (_currentState == State.Chase || _currentState == State.Patrol)
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
		// If we are damaged, try and find the source by increasing the awareness area
		if (_currentState == State.Patrol)
		{
			_navAgent.TargetPosition = GetNode<Node3D>("/root/Root/Player").Position;
			_currentState = State.Chase;
		}
			
		if (_health > 0)
			_health -= amount;
		
		if(_health <= 0)
			QueueFree();
	}

	private void Move(float delta)
	{
		var nextLocation = _navAgent.GetNextPathPosition();
		var direction = GlobalPosition.DirectionTo(nextLocation);
		
		LookAtFromPosition(Position, _navAgent.TargetPosition, Vector3.Up, true);
		
		Vector3 velocity = Velocity;
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * _speed;
			velocity.Z = direction.Z * _speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, _speed * delta);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, _speed * delta);
		}
		
		Velocity = velocity;
		MoveAndSlide();
		
		_animationTree.Set("parameters/run/blend_position", this.GetRealVelocity().Normalized().Length());
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
					break;
				case State.Idle:
					if (_patrolNodes.Count > 0)
					{
						_currentState = State.Patrol;
					}
					break;
				case State.Attack:
					if (_target == null)
					{
						if (_attackTriesCount >= 3)
						{
							_currentState = State.Patrol;
							_attackTriesCount = 0;
						}
						else
						{
							_currentState = State.Chase;
							_attackTriesCount++;
						}
						return;
					}
					
					if(!_swingRoutineRunning)
						SwingRoutine(delta);
					
					if (GetTargetDistance() > _attackDistance)
					{
						_currentState = State.Chase;
					}
					break;
				case State.Chase:
					if (GetTargetDistance() < _attackDistance)
					{
						_currentState = State.Attack;
						_attackTriesCount++;
					}
					break;
				case State.Patrol:
					if (GetTargetDistance() < 1f)
					{
						if (_currentPathIndex >= _patrolNodes.Count - 1)
							_currentPathIndex = 0;
						else
							_currentPathIndex++;
					}
					
					var node = (_patrolNodes[_currentPathIndex] as Node3D);
					_navAgent.TargetPosition = node.Position;
					
					break;
			}
		}
	}

	private async Task SwingRoutine(float delta)
	{
		_swingRoutineRunning = true;
		_animationTree.Set("parameters/punch/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);

		_currentState = State.AttackSwing;
		
		await Task.Delay(animAttackTime);
			
		OnCheckMeleeHurtbox();
		
		await Task.Delay(animTotalLength - animAttackTime);

		_currentState = !IsFacingTarget() ? State.Chase : State.Attack;
		_swingRoutineRunning = false;
	}

	private float GetTargetDistance()
	{
		return GlobalPosition.DistanceSquaredTo(_navAgent.TargetPosition);
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
	
	private void OnPlayerLost(Node3D body)
	{
		GD.Print($"I lost {body.Name}");

		_target = null;
		_navAgent.TargetPosition = Position;
		_currentState = State.Idle;
	}

	private bool IsFacingTarget()
	{
		var forwardVector = GlobalTransform.Basis.Z.Normalized();
		// var targetForwardVector = ;

		var dot = forwardVector.Dot(_navAgent.TargetPosition);
		return dot > 0.25f;
	}
}