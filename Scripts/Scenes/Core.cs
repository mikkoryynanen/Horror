using Godot;
using System;
using Horror.Scripts.Autoload;

public partial class Core : Node3D
{
	public override void _Ready()
	{
		AudioManager.Instance.PlayLevelMusic("res://Assets/music/obsidian/22J01_Obsidian005_Ambience_WAV16bit.wav");
	}

	public override void _Process(double delta)
	{
		if (Input.IsKeyPressed(Key.B))
		{
			var mode = DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed
				? DisplayServer.WindowMode.Fullscreen
				: DisplayServer.WindowMode.Windowed;
			DisplayServer.WindowSetMode(mode);
		}
	}
}
