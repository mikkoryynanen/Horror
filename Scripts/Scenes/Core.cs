using Godot;
using System;
using Horror.Scripts.Autoload;

public partial class Core : Node3D
{
	private GodotObject _musicNode;
	private string _currentIntensity = "Basic";

	public override void _Ready()
	{
		_musicNode = GetNode<GodotObject>("/root/Root/AudioPlayer");
		_musicNode.Call("play_music");
		
		Input.MouseMode = Input.MouseModeEnum.Captured;
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
		
		if (Input.IsActionJustPressed("interact"))
		{
			_currentIntensity = _currentIntensity == "High" ? "Basic" : "High";
			_musicNode.Call("on_set_intensity", _currentIntensity);
		}
	}
}
