using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.UI;

public partial class GUIManager : CanvasLayer
{
	private BoxContainer _interactionLabelContainer;
	private TextureRect _recticle;
	private AnimationPlayer _textPopupAnimationPlayer;
	private InventoryUI _inventory;
	private Label _textPopupLabel;
	private Control _pauseMenu;

	private Queue<string> _toastQueue = new();
	private Slider _staminaMeter;
	private Label _ammoLabel;
	private GridContainer _inputActionParent;
	private Slider _healthMeter;
	private ColorRect _lowHealthWarning;

	public override void _Ready()
	{
		_interactionLabelContainer = GetNode<BoxContainer>("Container/Reticle/InteractionLabelContainer");
		_recticle = GetNode<TextureRect>("Container/Reticle");
		_inventory = GetNode<InventoryUI>("Container/Inventory");
		_textPopupAnimationPlayer = GetNode<AnimationPlayer>("Container/TextPopup/AnimationPlayer");
		_textPopupLabel = GetNode<Label>("Container/TextPopup");
		_healthMeter = GetNode<Slider>("Container/MetersContainer/HealthMeter");
		_lowHealthWarning = GetNode<ColorRect>("Container/LowHealth");
		_staminaMeter = GetNode<Slider>("Container/MetersContainer/StaminaMeter");
		_ammoLabel = GetNode<Label>("Container/AmmoLabel");
		
		_pauseMenu = GetNode<Control>("Container/PauseMenu");
		_inputActionParent = GetNode<GridContainer>("Container/PauseMenu/Container/BoxContainer/KeybindsContainer");

		var signalBus = this.GetSignalBus();
		signalBus.OnShowInteract += OnShowInteract;
		signalBus.OnHideInteract += OnHideInteract;
		signalBus.OnActivatePlayerCamera += () => _recticle.Visible = true;
		signalBus.OnDeactivatePlayerCamera += () => _recticle.Visible = false;
		signalBus.OnItemPickedUp += OnItemPickup;
		signalBus.OnUpdateAmmo += OnUpdateAmmo;
		signalBus.OnMenuCancel += OnPause;
		
		signalBus.OnOpenInventory += () => _inventory.Visible = true;
		signalBus.OnCloseInventory += () => _inventory.Visible = false;
		
		signalBus.OnUpdateStamina += OnUpdateStamina;
		signalBus.OnUpdateHealth += OnUpdateHealth;

		OnHideInteract();
		
		_pauseMenu.Visible = false;
	}

	public override void _Process(double delta)
	{
		// TODO This gets run immidiately after when open inventory
		if (_inventory.Visible && Input.IsActionJustPressed("cancel"))
		{
			AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIConfirm);
			_inventory.Visible = false;
			
			this.EmitSignalBus("OnCloseInventory");
		}
		// else if (!_inventory.Visible && Input.IsActionJustPressed("inventory"))
		// {
		// 	// Inventory.Show();
		// 	AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIConfirm);
		// 	_inventory.Visible = true;
		// 	
		// 	this.EmitSignalBus("OnOpenInventory");
		// }
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
		_interactionLabelContainer.Visible = false;
	}

	private void OnShowInteract()
	{
		_interactionLabelContainer.Visible = true;
	}
	
	private void OnUpdateStamina(float value)
	{
		_staminaMeter.Value = value * 100;
	}
	
	private void OnUpdateHealth(float value)
	{
		_healthMeter.Value = value;
		if (value < 25)
		{
			var tween = GetTree().CreateTween();
			var valueDelta = 100 - value;
			tween.TweenProperty(_lowHealthWarning.Material, "shader_parameter/vignette_opacity", valueDelta / 100, 0.25f);
		}
	}
	
	private void OnUpdateAmmo(int currentAmmo, int totalAmmo)
	{
		// _ammoLabel.Visible = currentAmmo != 0 && totalAmmo != 0;
		_ammoLabel.Text = $"{currentAmmo} / {totalAmmo}";
	}
	
	private void OnPause()
	{
		if (_inventory.Visible) return;
		
		_pauseMenu.Visible = !_pauseMenu.Visible;

		if (_pauseMenu.Visible)
		{
			foreach (var actionNode in _inputActionParent.GetChildren())
				actionNode.QueueFree();

			var inputActions = InputMap.GetActions().Where(x => !x.ToString().Contains("ui_"));
			foreach (var action in inputActions)
			{
				var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/InputActionTemplate.tscn");
				var instance = packed.Instantiate() as BoxContainer;
				_inputActionParent.AddChild(instance);

				instance.GetNode("ControllerTextureRect").Set("path", action);
				instance.GetNode<Label>("Label").Text = action;
			}
		}
	}
}