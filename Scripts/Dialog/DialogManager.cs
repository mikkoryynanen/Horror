using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using DialogueManagerRuntime;
using Horror.Scripts;
using Horror.Scripts.Dialog;

public partial class DialogManager : Control
{
	// private Label _characterLabel;
	// private RichTextLabel _dialogLabel;
	// private Node _responsesParent;
	[Export()] private DialogMenu _dialogBalloon;
	[Export()] private DialogMenu _topDialogBalloon;

	private DialogMenu _currentDialogMenu;
	
	private AudioStreamPlayer _audioPlayer;
	private List<Button> _builtButtons = new();
	private DialogueLine _dialogLine;
	private Resource _dialogResource;
	private bool _responseGiven = false;
	private bool _waitingForInput = false;
	// private TextureRect _continueDialog;
	
	Dictionary<string, DialogueLine> _heardDialogs = new();

	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		
		_audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		// _characterLabel = GetNode<Label>("DialogBalloon/Margin/Balloon/CharacterLabel");
		// _dialogLabel = GetNode<RichTextLabel>("DialogBalloon/Margin/Balloon/Margin/VBox/DialogueLabel");
		// _responsesParent = GetNode<Node>("DialogBalloon/Margin/Balloon/Margin/VBox/Margin/Responses");
		_dialogBalloon = GetNode<DialogMenu>("DialogBalloon");
		_topDialogBalloon = GetNode<DialogMenu>("TopBalloon");
		// _continueDialog = GetNode<TextureRect>("DialogBalloon/Margin/Balloon/ContinueIndicator");

		_dialogBalloon.Visible = false;
		_topDialogBalloon.Visible = false;
		
		this.GetSignalBus().OnStartDialog += (act, title, isFullscren) => ProcessAct(act, title, isFullscren);
	}

	public override void _Process(double delta)
	{
		if (_waitingForInput && Input.IsActionJustPressed("interact"))
		{
 			NextDialog(_dialogLine.NextId);
		}
	}

	private async void ProcessAct(string act, string title, bool isFullscreenDialog)
	{
		_currentDialogMenu = isFullscreenDialog ? _dialogBalloon : _topDialogBalloon;
		
		Engine.GetSingleton("DialogueManager").Connect("dialogue_ended", new Callable(this, "OnDialogueEnded"));

		if (isFullscreenDialog)
			_dialogBalloon.Visible = true;
		else
			_topDialogBalloon.Visible = true;

		var dialogPath = $"res://dialogue/{act}/{title}.dialogue";
		_dialogResource = GD.Load<Resource>(dialogPath);
		
		_dialogLine = await DialogueManager.GetNextDialogueLine(_dialogResource, title);
		
		while (true)
		{
			while (_waitingForInput)
			{
				// _continueDialog.Visible = true;
				await Task.Delay(10);
			}
			
			Input.MouseMode = isFullscreenDialog ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured;
			_responseGiven = false;
			
			foreach (var button in _builtButtons)
				button.Free();
			_builtButtons.Clear();

			_currentDialogMenu.dialogLabel.Text = _dialogLine.Character;
			_currentDialogMenu.dialogLabel.Text = _dialogLine.Text;
			_currentDialogMenu.dialogLabel.Set("dialogue_line", _dialogLine);
			
			var dialogHeard = _heardDialogs.ContainsKey(_dialogLine.Text);
			if (!dialogHeard)
			{
				// Voice over
				var audioPath =
					$"res://Assets/dialog/{act}/vo_{_dialogLine.Character.ToLower()}_{title}_{_dialogLine.TranslationKey}.mp3";
				var stream = GD.Load<AudioStream>(audioPath);
				if (stream != null)
				{
					_audioPlayer.Stream = stream;
					_audioPlayer.Play();
				}
				
				_heardDialogs.Add(_dialogLine.Text, _dialogLine);
			}
			
			// Set text speed depending if it has been read before
			_currentDialogMenu.dialogLabel.Set("seconds_per_step", dialogHeard ? 0 : 0.04);
			
			// Text
			if (!string.IsNullOrEmpty(_dialogLine.Text))
			{
				_currentDialogMenu.dialogLabel.Call("type_out");
				await ToSignal(_currentDialogMenu.dialogLabel, "finished_typing");
			}
			
			foreach (var response in _dialogLine.Responses)
			{
				var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/dialog/response_button.tscn");
				var button = packed.Instantiate() as Button;

				button.Text = response.Text;
				button.Pressed += () =>
				{
					_responseGiven = true;

					Input.MouseMode = Input.MouseModeEnum.Captured;

					NextDialog(response.NextId);
				};
			
				_currentDialogMenu.responsesParent.AddChild(button);
				_builtButtons.Add(button);
			}

			// Wait for input
			if (_dialogLine.Responses.Count <= 0)
			{
				float time = _dialogLine.Text.Length * 0.02f;
				await ToSignal(GetTree().CreateTimer(time), "timeout");
				_waitingForInput = true;
			}
			else if (!string.IsNullOrEmpty(_dialogLine.Time))
			{
				float time = 0f;
				if (!float.TryParse(_dialogLine.Time, out time))
				{
					time = _dialogLine.Text.Length * 0.02f;
				}
				await ToSignal(GetTree().CreateTimer(time), "timeout");
				NextDialog(_dialogLine.NextId);
			}
			else
			{
				while (!_responseGiven)
				{
					await Task.Delay(10);
				}
			}
		}
	}

	private async void NextDialog(string nextId)
	{
		_dialogLine = await DialogueManager.GetNextDialogueLine(_dialogResource, nextId);
	}
	
	private void OnDialogueEnded(Resource resource)
	{
		this.EmitSignalBus("OnEndDialog");

		_dialogBalloon.Visible = false;
	}
}
