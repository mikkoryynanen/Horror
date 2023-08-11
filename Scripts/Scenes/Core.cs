using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Inventory;
using Horror.Scripts.UI;

namespace Horror.Scripts.Scenes;

public partial class Core : Node3D
{
	private GodotObject _musicNode;
	private bool _cutsceneRun;
	private string _currentRoom;
	
	public PlayerInventory Inventory { get; private set; }

	public override void _Ready()
	{
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

		if (_cutsceneRun && Input.IsAnythingPressed())
		{
			InputManager.Instance.SetProcessState(true);
		}
	}
	
	public void Init(string currentRoom, bool hasIntro)
	{
		_currentRoom = currentRoom;
		AudioManager.Instance.PlayLevelMusic();

		this.GetSignalBus().OnHideLetterbox += SpawnPlayer;

		if (hasIntro)
		{
			this.EmitSignalBus(nameof(SignalBus.OnStartCutscene));
		}
		else
		{
			SpawnPlayer();	
		}
	}
	
	private void SpawnPlayer()
	{
		var spawnPoint = GetNode<Node3D>($"{_currentRoom}/PlayerSpawnpoint");
		if (spawnPoint == null)
		{
			GD.PrintErr("Could not find player spawnpoint");
			return;
		}

		Vector3 spawnPosition, spawnRotation;
		spawnPosition = spawnPoint.Position;
		spawnRotation = spawnPoint.Rotation;
		
		// spawnPoint.QueueFree();
		
		var player = Loader.Instantiate<global::Player>("res://Scenes/Player.tscn");
		AddChild(player);

		Inventory = new PlayerInventory(player, GetNode<GUIManager>("GUI").GetInventoryUI());
		player.Position = spawnPosition;
		player.Rotation = spawnRotation;
		player.LookAt(spawnPosition, Vector3.Up, true);

		_cutsceneRun = true;
	}
}