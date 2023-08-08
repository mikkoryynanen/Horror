using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Scenes;

public partial class Core : Node3D
{
	private GodotObject _musicNode;
	private GridMap _gridmap;
	

	public override void _Ready()
	{
		AudioManager.Instance.PlayLevelMusic();
		
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
	}

	public void LoadRoom(string roomPath)
	{
		// Loader.Load(roomPath);
	}
}