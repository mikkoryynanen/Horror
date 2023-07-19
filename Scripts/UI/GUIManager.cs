using Godot;
using Horror.Scripts;

public partial class GUIManager : CanvasLayer
{
	private Label _interactLabel;
	
	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("Container/Reticle/Label");
		
		this.GetSignalBus().OnShowInteract += OnShowInteract;
		this.GetSignalBus().OnHideInteract += OnHideInteract;
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
