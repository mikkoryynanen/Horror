using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Inventory;

public class PlayerInventory : Inventory
{
    public List<Item> Weapons { get; private set; } = new();
    
    private int _currentEquippedWeaponIdx = 0;
    public Item CurrentEquippedWeapon => Weapons.Count > 0 ? Weapons[_currentEquippedWeaponIdx] : null;

    public PlayerInventory(Node node, InventoryUI ui) : base(ui)
    {
        node.GetSignalBus().OnItemPickedUp += itemId =>
        {
            var id = new Guid(itemId);
            var item = ItemDatabase.GetItem(id);
            if (item.ItemType == Item.Type.Weapon)
            {
                Weapons.Add(item);
            }
            else
                AddItem(item);
        };
    }

    public override void Show(string inventoryName = "Inventory")
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.UIClick);
        _ui.Build(this, "Inventory");
    }

    public void EquipNextWeapon()
    {
        if (_currentEquippedWeaponIdx < Weapons.Count - 1)
            _currentEquippedWeaponIdx++;
        else
            _currentEquippedWeaponIdx = 0;
    }
    
    public void EquipPreviousWeapon()
    {
        if (_currentEquippedWeaponIdx > 0)
            _currentEquippedWeaponIdx--;
        else
            _currentEquippedWeaponIdx = Weapons.Count - 1;
    }
}