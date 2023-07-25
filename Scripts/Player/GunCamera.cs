using Godot;
using System;
using System.Collections.Generic;
using Horror.Scripts;

public partial class GunCamera : Camera3D
{
	[Export()] private float intensity = 0.0005f;
	
	private Node3D _rig;
	private float _t;
	private AnimationPlayer _animator;

	public override void _Ready()
	{
		_rig = GetNode<Node3D>("Rig");
		_animator = GetNode<AnimationPlayer>("Rig/Pipe/AnimationPlayer");

		this.GetSignalBus().OnActivatePlayerCamera += Show;
		this.GetSignalBus().OnDeactivatePlayerCamera += PutAway;
	}

	public override void _Process(double delta)
	{
		_t = (float)delta * 20f;

		_rig.Position = new Vector3(
			Mathf.Lerp(_rig.Position.X, 0, _t), 
			Mathf.Lerp(_rig.Position.Y, 0, _t),
			Mathf.Lerp(_rig.Position.Z, 0, _t));
	}

	public void OnAnimationFinished(StringName animName)
	{
		if (animName == "melee")
		{
			_animator.Play("idle");
		}
	}

	public void Sway(Vector2 swayAmount)
	{
		_rig.Position += new Vector3(swayAmount.X * -intensity, swayAmount.Y * intensity, 0);
	}

	public void Shoot()
	{
		_animator.Play("melee");
	}

	public void PutAway()
	{
		_animator.Play("put_away");
	}
	
	public void Show()
	{
		_animator.Play("show");
	}
}
