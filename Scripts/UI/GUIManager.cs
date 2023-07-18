using Godot;
using System;
using Horror.Scripts.Autoload;

public partial class GUIManager : CanvasLayer
{
	private Label _interactLabel;
	
	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("Container/Reticle/Label");
		
		GetNode<SignalBus>("/root/SignalBus").OnShowInteract += OnShowInteract;
		GetNode<SignalBus>("/root/SignalBus").OnHideInteract += OnHideInteract;
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
