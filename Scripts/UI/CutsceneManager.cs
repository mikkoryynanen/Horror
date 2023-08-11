using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.UI;

public partial class CutsceneManager : CanvasLayer
{
	[Export()] private NodePath cutsceneObject;
	
	private AnimationPlayer _animationPlayer;
	private Control _root;
	private AnimationPlayer _cutscenePlayer;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("Control/AnimationPlayer");
		_root = GetNode<Control>("Control");

		var signalBus = this.GetSignalBus();
		signalBus.OnShowLetterbox += () =>
		{
			_root.Visible = true;
			_animationPlayer.Play("show");
		};
		signalBus.OnHideLetterbox += () =>
		{
			_animationPlayer.AnimationFinished += name =>
			{
				_root.Visible = false;
				InputManager.Instance.SetProcessState(false);
				this.EmitSignalBus(nameof(SignalBus.OnActivatePlayerCamera));
			};
			_animationPlayer.Play("hide");
		};
		signalBus.OnStartCutscene += ShowCutscene;
	}

	private void ShowCutscene()
	{	
		InputManager.Instance.SetProcessState(false);
		
		this.EmitSignalBus(nameof(SignalBus.OnShowLetterbox));
		
		_cutscenePlayer = GetNode<AnimationPlayer>($"{cutsceneObject}/AnimationPlayer");
		_cutscenePlayer.Play("cutscene");
		_cutscenePlayer.AnimationFinished += name => EndAnimation();
	}

	private void EndAnimation()
	{
		GetNode<Node>("Act_0_Room_0_Cutscene").QueueFree();

		this.EmitSignalBus(nameof(SignalBus.OnHideLetterbox));
		this.EmitSignalBus(nameof(SignalBus.OnDeactivatePlayerCamera));
	}
}