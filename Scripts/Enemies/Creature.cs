using Godot;
using System;
using Horror.Scripts.Autoload;

public partial class Creature : MeshInstance3D
{
	public void OnTrigger()
	{
		GetNode<AnimationPlayer>("AnimationPlayer").Play("trigger");
		AudioManager.Instance.PlayClip(GD.Load<AudioStream>("res://Assets/sfx/Cinematic/cinematic_1.wav"));
	}
}
