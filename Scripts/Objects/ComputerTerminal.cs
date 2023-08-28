using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.UI;

namespace Horror.Scripts.Objects;

public partial class ComputerTerminal : StaticBody3D, IInteractable
{
    private bool _isInteracting = false;
    private SubViewport _subViewport;
    private Camera3D _camera;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        _subViewport = GetNode<SubViewport>("Node/SubViewport");
    }

    public override void _Process(double delta)
    {
        if (_isInteracting && Input.IsActionJustPressed("cancel"))
        {
            Exit();
        }
    }

    public override void _Input(InputEvent inputEvent)
    {
        if(_isInteracting)
            _subViewport.PushInput(inputEvent);
    }

    private void Exit()
    {
        this.EmitSignalBus(nameof(SignalBus.OnHideInteract));
        this.EmitSignalBus(nameof(SignalBus.OnActivatePlayerCamera));
    
        _isInteracting = false;
    
        _camera.ClearCurrent();
    }

    public void Interact()
    {
        this.EmitSignalBus(nameof(SignalBus.OnHideInteract));
        this.EmitSignalBus(nameof(SignalBus.OnDeactivatePlayerCamera));
        
        _camera.MakeCurrent();
        
        _isInteracting = true;
            
        GetNode<Button>("Node/SubViewport/InfoTerminal/BackgroundColor/Margin/Panel/VBox/Button").GrabFocus();
        // GetNode<AnimationPlayer>("Camera3D/AnimationPlayer").Play("show");
    }

    public void HoldInteract()
    {
    }
}