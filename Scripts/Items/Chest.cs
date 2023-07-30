using Godot;
using Horror.Scripts.Inventory.Items;
using Horror.Scripts.UI;

namespace Horror.Scripts.Items;

public partial class Chest : StaticBody3D, IInteractable
{
    public Inventory.Inventory Inventory { get; private set; }
    
    public override void _Ready()
    {
        Inventory = new(GetNode<GUIManager>("/root/Root/GUI").GetInventoryUI());
        Inventory.AddItem(new Pipe());
    }

    public void Interact()
    {
        Inventory.Show("Chest");
        this.EmitSignalBus("OnOpenInventory");
    }
}