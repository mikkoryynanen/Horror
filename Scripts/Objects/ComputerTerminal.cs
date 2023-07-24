using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Objects;

public partial class ComputerTerminal : Node3D, IInteractable
{
    private bool _canInteract = false;
    private bool _isInteracting = false;
    private Camera3D _camera;
    private SubViewport _subViewport;

    public override void _Ready()
    {
        _camera = GetNode<Camera3D>("Camera3D");
        _subViewport = GetNode<SubViewport>("Node/SubViewport");
    }

    public override void _Process(double delta)
    {
        if (_canInteract && !_isInteracting && Input.IsActionJustPressed("interact"))
        {
            Interact();
        }
            
        if (_isInteracting && Input.IsKeyPressed(Key.Escape))
        {
            Exit();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if(_canInteract)
            _subViewport.PushInput(@event);
    }

    public void OnAreaEntered(Node3D node)
    {
        if (node.IsInGroup("Player"))
        {
            this.EmitSignalBus(nameof(SignalBus.OnShowInteract));
            
            GD.Print("player entered");
            _canInteract = true;
        }
    }
    
    public void OnAreaExited(Node3D node)
    {
        if (node.IsInGroup("Player"))
        {
            GD.Print("player exit");
            Exit();
        }
    }

    private void Exit()
    {
        this.EmitSignalBus(nameof(SignalBus.OnHideInteract));
        this.EmitSignalBus(nameof(SignalBus.OnActivatePlayerCamera));

        _canInteract = false;
        _isInteracting = false;

        _camera.ClearCurrent();
    }

    public void Interact()
    {
        this.EmitSignalBus(nameof(SignalBus.OnHideInteract));
        this.EmitSignalBus(nameof(SignalBus.OnDeactivatePlayerCamera));
        
        _camera.MakeCurrent();
        
        _isInteracting = true;
            
        GetNode<AnimationPlayer>("Camera3D/AnimationPlayer").Play("show");
    }
}