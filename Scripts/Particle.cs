using Godot;
using System;

public partial class Particle : CpuParticles3D
{
	public override void _Ready()
	{
		Emitting = true;
	}

	public override void _Process(double delta)
	{
		if (!Emitting)
			QueueFree();
	}
}
