using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Horror.Scripts.Scenes;

public partial class Loading : Control
{
	private float _timer = 0f;
	
	private Label _loadingLabel;
	private float _waitTime;
	private AnimationPlayer _animationPlayer;
	private bool _loadingCompleted;

	private const string CoreScenePath = "res://Scenes/Parent.tscn";

	private readonly Dictionary<string, IReadOnlyList<string>> _roomRelationships = new()
	{
		{CoreScenePath, new List<string>() { "res://Prefabs/Rooms/Act0/Room0/Act_0_Room_0.tscn" }}
	};

	public override void _Process(double delta)
	{
		// if (!_hasStarted)
		// {
		// 	GD.PrintErr("Loading screen .Start() has not been called");
		// 	return;
		// }
		
		if (!_loadingCompleted && _loadingLabel != null)
		{
			_timer += (float)delta;
			if (_timer >= _waitTime)
			{
				if (_loadingLabel.VisibleCharacters >= _loadingLabel.Text.Length)
					_loadingLabel.VisibleCharacters = 0;
				else
				{
					_loadingLabel.VisibleCharacters++;
					_waitTime = _loadingLabel.VisibleCharacters >= _loadingLabel.Text.Length ? 1f : .25f;
				}
				_timer = 0f;
			}
		}
	}

	/// <summary>
	/// Actually starts the loading process of the specified level. Without calling this method, loading will not start and game hangs forever
	/// </summary>
	public void Start(string scenePath, bool sceneHasIntroCutscene = false)
	{
		var rootScenePath = _roomRelationships.FindKeyByValue(scenePath);
		if (rootScenePath == null)
		{
			GD.PrintErr($"Could not find root scene for {scenePath}");
			return;
		}
		
		_loadingLabel = GetNode<Label>("Margin/Label");
		_animationPlayer = GetNode<AnimationPlayer>("ColorRect/AnimationPlayer");

		var rootScene = LoadScene(rootScenePath);
		var core = rootScene.GetNode<Core>("CanvasLayer/BlurPostProcess/Viewport/DitherBanding/Viewport/Core");
		LoadScene(scenePath, core);
		
		core.Init(Path.GetFileNameWithoutExtension(scenePath), sceneHasIntroCutscene);
		
		_animationPlayer.AnimationFinished += name =>
		{
			MenuManager.Instance.UnloadAllMenus();
			_loadingCompleted = true;
		};
		
		_animationPlayer.Play("show");
	}

	private Node LoadScene(string scenePath, Node parent = null)
	{
		float progress = 0;
		var sceneError = Loader.Load(scenePath);
		while (progress < 1f)
		{
			progress = (float)Loader.GetLoadingProgress(scenePath);
		}

		var loadedScene = Loader.GetLoadedScene(scenePath);
		if (loadedScene == null)
		{
			GD.PrintErr($"Failed to get loaded scene {scenePath}");
			return null;
		}

		return MenuManager.Instance.AddScene(loadedScene, parent);
	}
}