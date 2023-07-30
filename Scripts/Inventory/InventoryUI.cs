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

	public override void _Ready()
	{
		_itemParent = GetNode<GridContainer>("BG/Margin/Container/HBox/HBox/Grid");
		_weaponContainer = GetNode<VBoxContainer>("BG/Margin/Container/Weapon");
		_equippedWeapon = GetNode<InventoryItem>("BG/Margin/Container/Weapon/VBox/HBox/InventoryItem");
		_inventoryTitle = GetNode<Label>("BG/Margin/Container/InventoryTitle");
		_lootableInfo = GetNode<Label>("LootableInfo");
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
		
		if (playerInventory.EquippedWeapon != null)
			_equippedWeapon.Build(GD.Load<Texture2D>(playerInventory.EquippedWeapon.IconPath));
		
		PopulateInventory(playerInventory as Inventory);
	}

	private void PopulateInventory(Inventory inventory)
	{
		foreach (var item in _builtInventoryItems)
			item.Free();
		_builtInventoryItems.Clear();
		
		// TODO Add paging if needed
		var items = inventory.GetItems();
		for (int i = 0; i < items.Count; i++)
		{
			var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/UI/Inventory/InventoryItem.tscn");
			var inventoryItem = packed.Instantiate() as InventoryItem;
			_itemParent.AddChild(inventoryItem);
			
			inventoryItem.Build(GD.Load<Texture2D>(items[i].IconPath));
			
			_builtInventoryItems.Add(inventoryItem);
			
			// Always have the first one selected
			// TODO Should this only be with gamepad?
			if (i == 0)
				inventoryItem.OnSelect();
		}
	}

	public override void _Process(double delta)
	{
		// Weapon selection
		if (Input.IsActionJustPressed("right") && _currentSelectedWeaponIdx < _maxWeaponsCount)
			_currentSelectedWeaponIdx++;
		else if (Input.IsActionJustPressed("left") && _currentSelectedWeaponIdx > 0)
			_currentSelectedWeaponIdx--;
		
		// Close
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			this.EmitSignalBus("OnCloseInventory");
		}

		if (Input.IsActionJustPressed("interact"))
		{
			GD.Print("loot single");
		}
		
		if (Input.IsActionJustPressed("loot_all"))
		{
			GD.Print("loot all");
		}
	}
}