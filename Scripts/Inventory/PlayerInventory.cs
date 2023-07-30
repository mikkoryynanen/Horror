using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Horror.Scripts.Inventory;

public class PlayerInventory : Inventory
{
    public Item EquippedWeapon { get; private set; }

    private IReadOnlyList<Guid> _weaponIds = new[]
    {
        new Guid("007c369c-3e9a-4391-ba0a-aaad097563b7")
    };

    public PlayerInventory(Node node, InventoryUI ui) : base(ui)
    {
        node.GetSignalBus().OnItemPickedUp += itemId =>
        {
            var id = new Guid(itemId);
            var item = ItemDatabase.GetItem(id);
            if (!IsWeapon(id))
                AddItem(item);
            else
                EquippedWeapon = item;
        };
    }

    public override void Show(string inventoryName = "Inventory")
    {
        _ui.Build(this, "Inventory");
    }

    private bool IsWeapon(Guid itemId)
    {
        // Note Quick checker if item is matches one of the weapon ids
        // TODO This could be improved
        return _weaponIds.Contains(itemId);
    }
}