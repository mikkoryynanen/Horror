using Godot;

namespace Horror.Scripts.Autoload;

public partial class SignalBus : Node
{
    [Signal]
    public delegate void OnShowInteractEventHandler();
    [Signal]
    public delegate void OnHideInteractEventHandler();
    
    [Signal]
    public delegate void OnRechargeTorchlightEventHandler(float value);
    
    [Signal]
    public delegate void OnChangeTimescaleEventHandler(float newTimescale);
}