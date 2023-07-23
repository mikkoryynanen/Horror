using Godot;
using System.Collections.Generic;
using Horror.Scripts.StateMachine;

public partial class StateMachine : Node
{
	private Dictionary<string, State> _states = new();
	private State _currentState;
	
	public override void _Ready()
	{
		_states.Add("idle", new IdleState());
		_states.Add("attack", new AttackState());
		
		_currentState = _states["idle"];
		_currentState.Enter();
	}

	public void OnStateTransitioned(State newState)
	{
		_currentState = newState;
		GD.Print($"changed to state {nameof(newState)}");
	}

	public override void _Process(double delta)
	{
		if (_currentState != null)
			_currentState.Update(delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_currentState != null)
			_currentState.PhysicsUpdate(delta);
	}
}
