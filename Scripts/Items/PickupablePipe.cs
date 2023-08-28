using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.Items;

public partial class PickupablePipe : StaticBody3D, IInteractable
{
    public void Interact()
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.MeleePickup);
        
        this.EmitSignalBus("OnItemPickedUp", ItemDatabase.GetItem("Pipe").Id.ToString());
        
        QueueFree();
    }

    public void HoldInteract()
    {
        throw new System.NotImplementedException();
    }
}