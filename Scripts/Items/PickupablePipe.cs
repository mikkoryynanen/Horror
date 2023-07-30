using Godot;
using Horror.Scripts.Inventory;
using Horror.Scripts.Inventory.Items;

namespace Horror.Scripts.Items;

public partial class PickupablePipe : StaticBody3D, IInteractable
{
    private const string Id = "007c369c-3e9a-4391-ba0a-aaad097563b7";
    
    public void Interact()
    {
        this.EmitSignalBus("OnItemPickedUp", Id);
        
        QueueFree();
    }
}