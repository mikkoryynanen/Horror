using Godot;
using Horror.Scripts.Inventory.Items;
using Horror.Scripts.UI;

namespace Horror.Scripts.Interactables;

public partial class Cabinet : StaticBody3D, IInteractable
{
    public Inventory.Inventory Inventory { get; private set; }
    
    public override void _Ready()
    {
        Inventory = new(GetNode<GUIManager>("/root/Core/GUI").GetInventoryUI());
        Inventory.AddItem(new QuestItem());
    }

    public void Interact()
    {
        Inventory.Show("Cabinet");
        this.EmitSignalBus("OnOpenInventory");
    }
}