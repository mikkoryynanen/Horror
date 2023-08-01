using System;
using System.Collections.Generic;
using Godot;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.UI;

public partial class GUIManager : CanvasLayer
{
	private Label _interactLabel;
	private TextureRect _recticle;
	private AnimationPlayer _textPopupAnimationPlayer;
	private InventoryUI _inventory;
	private Label _textPopupLabel;

	private Queue<string> _toastQueue = new();
	private Slider _staminaMeter;

	public override void _Ready()
	{
		_interactLabel = GetNode<Label>("Container/Reticle/Label");
		_recticle = GetNode<TextureRect>("Container/Reticle");
		_inventory = GetNode<InventoryUI>("Container/Inventory");
		_textPopupAnimationPlayer = GetNode<AnimationPlayer>("Container/TextPopup/AnimationPlayer");
		_textPopupLabel = GetNode<Label>("Container/TextPopup");
		_staminaMeter = GetNode<Slider>("Container/Indicator");

		var signalBus = this.GetSignalBus();
		signalBus.OnShowInteract += OnShowInteract;
		signalBus.OnHideInteract += OnHideInteract;
		signalBus.OnActivatePlayerCamera += () => _recticle.Visible = true;
		signalBus.OnDeactivatePlayerCamera += () => _recticle.Visible = false;
		signalBus.OnItemPickedUp += OnItemPickup;
		signalBus.OnOpenInventory += () => _inventory.Visible = true;
		signalBus.OnCloseInventory += () => _inventory.Visible = false;
		signalBus.OnUpdateStamina += OnUpdateStamina;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (_toastQueue.Count > 0 && !_textPopupAnimationPlayer.IsPlaying())
		{
			if (_toastQueue.TryDequeue(out var message))
			{
				_textPopupLabel.Text = message;
				_textPopupAnimationPlayer.Play("show");
			}
		}
	}

	public InventoryUI GetInventoryUI()
	{
		return _inventory;
	}

	private void OnItemPickup(string itemId)
	{
		var item = ItemDatabase.GetItem(new Guid(itemId));
		_toastQueue.Enqueue($"{item?.Name} picked up");
		GD.Print($"{item?.Name} picked up");
	}

	private void OnHideInteract()
	{
		_interactLabel.Visible = false;
	}

	private void OnShowInteract()
	{
		_interactLabel.Visible = true;
	}
	
	private void OnUpdateStamina(float value)
	{
		_staminaMeter.Value = value * 100;
	}
}