using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Horror.Scripts.Inventory;

public class PlayerInventory : Inventory
{
    public List<Item> EquippedWeapons { get; private set; } = new();

    private IReadOnlyList<Guid> _weaponIds = new[]
    {
        new Guid("189eac2e-f918-43fb-a5c4-c9f40202bbed")
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
                EquippedWeapons.Add(item);
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