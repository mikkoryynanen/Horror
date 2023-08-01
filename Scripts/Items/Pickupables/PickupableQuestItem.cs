using Godot;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.Items.Pickupables;

public partial class PickupableQuestItem  : StaticBody3D, IInteractable
{
    public void Interact()
    {
        this.EmitSignalBus("OnItemPickedUp", ItemDatabase.GetItem("Quest item").Id.ToString());
        
        QueueFree();
    }
}