using System;
using System.Collections.Generic;
using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Inventory;

public partial class InventoryUI : Control
{
	private GridContainer _itemParent;
	private VBoxContainer _weaponContainer;
	private InventoryItem _equippedWeapon;
	private PlayerInventory _playerInventory;
	
	private List<InventoryItem> _builtInventoryItems = new();
	private Label _inventoryTitle;
	private Label _lootableInfo;
	private Label _selectedItemTechnicalTextLabel;
	private Label _selectedItemNameLabel;

	public override void _Ready()
	{
		_itemParent = GetNode<GridContainer>("BG/Margin/Container/HBox/HBox/Grid");
		_weaponContainer = GetNode<VBoxContainer>("BG/Margin/Container/Weapon");
		_equippedWeapon = GetNode<InventoryItem>("BG/Margin/Container/Weapon/VBox/HBox/InventoryItem");
		_inventoryTitle = GetNode<Label>("BG/Margin/Container/InventoryTitle");
		_lootableInfo = GetNode<Label>("LootableInfo");
		_selectedItemTechnicalTextLabel = GetNode<Label>("SelectedItemInfo/ItemTechnicalText");
		_selectedItemNameLabel = GetNode<Label>("SelectedItemInfo/ItemName");
	}

	public override void _Process(double delta)
	{
		if (!Visible) return;
		
		// Weapon selection
		if (_playerInventory?.Weapons.Count > 1)
		{
			if (Input.IsActionJustPressed("right"))
			{
				AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIConfirm);
				_playerInventory.EquipNextWeapon();
				BuildWeaponSlot();
				
				this.EmitSignalBus("OnChangeWeapon", _playerInventory.CurrentEquippedWeapon.Id.ToString());
			}
			else if (Input.IsActionJustPressed("left"))
			{
				AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIConfirm);
				_playerInventory.EquipPreviousWeapon();
				BuildWeaponSlot();
				
				this.EmitSignalBus("OnChangeWeapon", _playerInventory.CurrentEquippedWeapon.Id.ToString());
			}	
		}
		
		// Close
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIConfirm);
			this.EmitSignalBus("OnCloseInventory");
		}

		if (Input.IsActionJustPressed("interact"))
		{
			// GD.Print("loot single");
		}
		
		if (Input.IsActionJustPressed("loot_all"))
		{
			// GD.Print("loot all");
		}
	}

	public void Build(Inventory inventory, string inventoryName)
	{
		_lootableInfo.Visible = true;
		_weaponContainer.Visible = false;
		_inventoryTitle.Text = inventoryName;

		PopulateInventory(inventory);
	}

	public void Build(PlayerInventory playerInventory, string inventoryName)
	{
		_playerInventory = playerInventory;
		_lootableInfo.Visible = false;
		_weaponContainer.Visible = true;
		_inventoryTitle.Text = "Inventory";
		
		BuildWeaponSlot();
		
		PopulateInventory(playerInventory as Inventory);
	}

	private void BuildWeaponSlot()
	{
		if (_playerInventory?.CurrentEquippedWeapon == null) return;
		
		_equippedWeapon.Build(GD.Load<Texture2D>(_playerInventory.CurrentEquippedWeapon.IconPath));
		GD.Print($"Currently equipped weapon {_playerInventory.CurrentEquippedWeapon.Name}");
	}

	private void PopulateInventory(Inventory inventory)
	{
		foreach (var item in _builtInventoryItems)
			item.Free();
		_builtInventoryItems.Clear();
		
		// TODO Add paging if needed
		var items = inventory.GetItems();
		
		// 9 is the max slots we can allow per page
		for (int i = 0; i < 9; i++)
		{
			var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/Inventory/InventoryItem.tscn");
			var inventoryItem = packed.Instantiate() as InventoryItem;
			_itemParent.AddChild(inventoryItem);

			inventoryItem.Build(i < items.Count ? GD.Load<Texture2D>(items[i].IconPath) : null);

			_builtInventoryItems.Add(inventoryItem);
			
			// Always have the first one selected
			// TODO Should this only be with gamepad?
			if (i == 0 && items.Count > 0)
				OnSelectItem(inventoryItem, items[i]);
		}
	}

	private void OnSelectItem(InventoryItem inventoryItem, Item item)
	{
		inventoryItem.OnSelect();
		_selectedItemNameLabel.Text = item.Name;
		_selectedItemTechnicalTextLabel.Text = item.Description;
	}
}