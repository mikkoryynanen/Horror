using Godot;

namespace Horror.Scripts.Items.Pickupables;

public partial class PickupableQuestItem  : StaticBody3D, IInteractable
{
    private const string Id = "82c4b2a1-6700-4d6f-a277-f26ca6bc4f8f";
    
    public void Interact()
    {
        this.EmitSignalBus("OnItemPickedUp", Id);
        
        QueueFree();
    }
}