using System;
using Godot;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.UI;

public partial class GUIManager : CanvasLayer
{
	private Label _interactLabel;
	private TextureRect _recticle;
	private AnimationPlayer _textPopupAnimationPlayer;
	private InventoryUI _inventory;

	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("Container/Reticle/Label");
		_recticle = GetNode<TextureRect>("Container/Reticle");
		_textPopupAnimationPlayer = GetNode<AnimationPlayer>("Container/TextPopup/AnimationPlayer");
		_inventory = GetNode<InventoryUI>("Container/Inventory");
		
		this.GetSignalBus().OnShowInteract += OnShowInteract;
		this.GetSignalBus().OnHideInteract += OnHideInteract;
		this.GetSignalBus().OnActivatePlayerCamera += () => _recticle.Visible = true;
		this.GetSignalBus().OnDeactivatePlayerCamera += () => _recticle.Visible = false;
		this.GetSignalBus().OnItemPickedUp += OnItemPickup;
		this.GetSignalBus().OnOpenInventory += () => _inventory.Visible = true;
		this.GetSignalBus().OnCloseInventory += () => _inventory.Visible = false;
	}

	public InventoryUI GetInventoryUI()
	{
		return _inventory;
	}

	private void OnItemPickup(string itemId)
	{
		_textPopupAnimationPlayer.Play("show");
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