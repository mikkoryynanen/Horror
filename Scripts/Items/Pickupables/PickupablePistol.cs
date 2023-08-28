using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Inventory;

namespace Horror.Scripts.Items.Pickupables;

public partial class PickupablePistol : StaticBody3D, IInteractable
{
    public void Interact()
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.MeleePickup);
        
        this.EmitSignalBus("OnItemPickedUp", ItemDatabase.GetItem("Pistol").Id.ToString());
        
        QueueFree();
    }

    public void HoldInteract()
    {
    }
}