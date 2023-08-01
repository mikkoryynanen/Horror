using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Items;

public partial class PickupablePipe : StaticBody3D, IInteractable
{
    private const string Id = "189eac2e-f918-43fb-a5c4-c9f40202bbed";
    
    public void Interact()
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.MeleePickup);
        
        this.EmitSignalBus("OnItemPickedUp", Id);
        
        QueueFree();
    }
}