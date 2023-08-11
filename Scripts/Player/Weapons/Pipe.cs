using Godot;

namespace Horror.Scripts.Player.Weapons;

public partial class Pipe : MeleeBase
{
    public override void _Ready()
    {
        base._Ready();
        
        var signalBus = this.GetSignalBus();
        signalBus.OnDeactivatePlayerCamera += () =>
        {
            GetNode<Camera3D>("Camera").ClearCurrent();
        };
        signalBus.OnActivatePlayerCamera += () =>
        {
            GetNode<Camera3D>("Camera").MakeCurrent();
        };
    }
}