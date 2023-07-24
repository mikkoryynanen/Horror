using Godot;

namespace Horror.Scripts.UI;

public partial class GUIManager : CanvasLayer
{
	private Label _interactLabel;
	private TextureRect _recticle;
	
	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("Container/Reticle/Label");
		_recticle = GetNode<TextureRect>("Container/Reticle");
		
		this.GetSignalBus().OnShowInteract += OnShowInteract;
		this.GetSignalBus().OnHideInteract += OnHideInteract;
		this.GetSignalBus().OnActivatePlayerCamera += () => _recticle.Visible = true;
		this.GetSignalBus().OnDeactivatePlayerCamera += () => _recticle.Visible = false;
	}

	private void OnHideInteract()
	{
		_interactLabel.Visible = false;
	}

	private void OnShowInteract()
	{
		_interactLabel.Visible = true;
	}
}