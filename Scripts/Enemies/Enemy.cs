using Godot;
using Horror.Scripts.Damage;

public partial class Enemy : Node3D
{
	private Node3D _target;
	private NavigationAgent3D _navAgent;
	private Rid _navMap;

	private enum State { None, Idle, Attack }

	private State _currentState = State.Idle;

	public override void _Ready()
	{
		_target = GetNode<Node3D>("/root/Core/Player");
		_navAgent = GetNode<NavigationAgent3D>("NavigationAgent");
	
		_navAgent.PathDesiredDistance = 0.25f;

		_navMap = GetNode<RoomManager>("/root/Core/RoomManager").GetNavigationMap();
		_navAgent.SetNavigationMap(_navMap);
	}

	public override void _PhysicsProcess(double delta)
	{
		_navAgent.TargetPosition = _target.GlobalPosition;
			
		if (_navAgent.IsTargetReachable() && !_navAgent.IsTargetReached())
		{
			var nextLocation = _navAgent.GetNextPathPosition();
			var direction = GlobalPosition.DirectionTo(nextLocation);
			GlobalPosition += direction * (float)delta;
		}

		if (_currentState == State.Attack)
		{
			if (_target is IDamageable damageable)
			{
				damageable.TakeDamage(10);
			}
		}
	}

	public void OnTargetReached()
	{
		_currentState = State.Attack;
	}
}
