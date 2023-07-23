using Godot;
using System;

public partial class Core : Node3D
{
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
