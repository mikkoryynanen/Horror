using Godot;
using System;

public partial class Flash : MeshInstance3D
{
	private AnimationPlayer _anim;

	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_anim.IsPlaying())
		{
			QueueFree();
		}
	}
}
