using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Horror.Scripts.Inventory;

public class Inventory
{
    protected readonly InventoryUI _ui;
    private Dictionary<Guid, Item> _inventory = new();

    public Inventory(InventoryUI ui)
    {
        _ui = ui;
    }
    
    public virtual void Show(string inventoryName = "Inventory")
    {
        _ui.Build(this, inventoryName);
    }
    
    public void AddItem(Item item)
    {
        try
        {
            _inventory.TryAdd(item.Id, item);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }
    }
    
    public Item GetItem(Guid itemId)
    {
        return _inventory.TryGetValue(itemId, out var item) ? item : null;
    }
    
    /// <summary>
    /// Gets all of the items in the inventory
    /// </summary>
    public IReadOnlyList<Item> GetItems()
    {
        return _inventory.Values.ToList();
    }
}