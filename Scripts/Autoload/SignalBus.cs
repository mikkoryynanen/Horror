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
    public delegate void OnUpdateAmmoEventHandler(int currentAmmo, int totalAmmo);
    
    [Signal]
    public delegate void OnChangeWeaponEventHandler(string weaponId);
    
    [Signal]
    public delegate void OnCameraShakeEventHandler(float trauma);
    [Signal]
    public delegate void OnCameraShakeContinuousEventHandler(float trauma);
    [Signal]
    public delegate void OnCameraShakeStopEventHandler();
    
    [Signal]
    public delegate void OnStartCutsceneEventHandler();
    
    // UI
    [Signal]
    public delegate void OnMenuCancelEventHandler();
    [Signal]
    public delegate void OnUpdateStaminaEventHandler(float value);
    [Signal]
    public delegate void OnReduceStaminaEventHandler(float value);
    [Signal]
    public delegate void OnUpdateHealthEventHandler(float value);
    [Signal]
    public delegate void OnShowLetterboxEventHandler();
    [Signal]
    public delegate void OnHideLetterboxEventHandler();
    [Signal]
    public delegate void OnPryEventHandler();
    [Signal]
    public delegate void OnPryEndEventHandler();
    
    // Game Events
    [Signal]
    public delegate void OnShipAlarmStartEventHandler();
    [Signal]
    public delegate void OnShipAlarmStopEventHandler();
}