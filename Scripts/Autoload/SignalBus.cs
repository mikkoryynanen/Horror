using System;
using Godot;

namespace Horror.Scripts.Autoload;

public partial class SignalBus : Node
{
    [Signal]
    public delegate void OnShowInteractEventHandler();
    [Signal]
    public delegate void OnHideInteractEventHandler();
    
    [Signal]
    public delegate void OnItemPickedUpEventHandler(string itemId);
    
    [Signal]
    public delegate void OnRechargeTorchlightEventHandler(float value);
    
    [Signal]
    public delegate void OnChangeTimescaleEventHandler(float newTimescale);
    
    [Signal]
    public delegate void OnStartDialogEventHandler(string act, string title, bool isFullscreenDialog);
    [Signal]
    public delegate void OnEndDialogEventHandler();

    [Signal]
    public delegate void OnActivatePlayerCameraEventHandler();
    [Signal]
    public delegate void OnDeactivatePlayerCameraEventHandler();
    
    [Signal]
    public delegate void OnMoodIncreaseEventHandler();
    [Signal]
    public delegate void OnMoodDecreaseEventHandler();
    
    [Signal]
    public delegate void OnOpenInventoryEventHandler();
    [Signal]
    public delegate void OnCloseInventoryEventHandler();
    
    [Signal]
    public delegate void OnUpdateStaminaEventHandler(float value);
    
}