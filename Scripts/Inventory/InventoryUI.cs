using System.Collections.Generic;
using Godot;

namespace Horror.Scripts.Inventory;

public partial class InventoryUI : Control
{
	private GridContainer _itemParent;
	private VBoxContainer _weaponContainer;
	private InventoryItem _equippedWeapon;
	
	private int _currentSelectedWeaponIdx = 0;
	private int _maxWeaponsCount = 3;
	private List<InventoryItem> _builtInventoryItems = new();
	private Label _inventoryTitle;
	private Label _lootableInfo;
	private Label _selectedItemTechnicalTextLabel;
	private Label _selectedItemNameLabel;
	private List<Item> _equippedWeapons;

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
		if (Input.IsActionJustPressed("right") && _currentSelectedWeaponIdx < _maxWeaponsCount)
		{
			_currentSelectedWeaponIdx++;
			BuildWeaponSlot();
		}
		else if (Input.IsActionJustPressed("left") && _currentSelectedWeaponIdx > 0)
		{
			_currentSelectedWeaponIdx--;
			BuildWeaponSlot();
		}
		
		// Close
		if (Input.IsActionJustPressed("ui_cancel"))
		{
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
		_lootableInfo.Visible = false;
		_weaponContainer.Visible = true;
		_inventoryTitle.Text = "Inventory";

		_maxWeaponsCount = playerInventory.EquippedWeapons.Count - 1;
		_currentSelectedWeaponIdx = 0;
		_equippedWeapons = playerInventory.EquippedWeapons;
		BuildWeaponSlot();
		
		PopulateInventory(playerInventory as Inventory);
	}

	private void BuildWeaponSlot()
	{
		if (_equippedWeapons.Count > 0)
			_equippedWeapon.Build(
				GD.Load<Texture2D>(
					_equippedWeapons[_currentSelectedWeaponIdx].IconPath));
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