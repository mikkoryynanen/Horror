using Godot;

namespace Horror.Scripts.Autoload;

public partial class InputManager : Node
{
    public static InputManager Instance => _instance;
    private static InputManager _instance;

    private Vector2 _lookRotation;

    private float _minLookAngle = -80;
    private float _maxLookAngle = 90;
    
    private float _mouseSensitivity = 0.2f;
    private Vector2 _mouseRelative;

    private float _joypadSensitivity = 0.35f;
    private float _joypadDeadzone = 0.5f;
    private Vector2 _joypadAxis;
    private InputEvent _lastInputEvent;

    private enum InputType {
        KeyboardMouse,
        Controller
    }

    private InputType _currentInputType;


    public override void _EnterTree(){
        if(_instance != null){
            this.QueueFree();
        }
        _instance = this;
    }

    public override void _Ready()
    {
        GetNode("/root/ControllerIcons").Connect("input_type_changed", new Callable(this, "OnControllerTypeChanged"));
    }

    private void OnControllerTypeChanged(InputType inputType)
    {
        _currentInputType = inputType;
    }

    public override void _Process(double delta)
    {
        _joypadAxis = new (Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));

        if (Input.IsActionJustPressed("cancel"))
        {
            this.EmitSignalBus("OnMenuCancel");
        }
    }
    
    public override void _Input(InputEvent inputEvent)
    {
        if (_currentInputType == InputType.KeyboardMouse)
        {
            if (inputEvent is InputEventMouseMotion motion)
            {
                Vector2 lookVector = _lookRotation;
                lookVector.Y -= motion.Relative.X * _mouseSensitivity;
                lookVector.X -= motion.Relative.Y * _mouseSensitivity;
                lookVector.X = Mathf.Clamp(lookVector.X, _minLookAngle, _maxLookAngle);
                _lookRotation = lookVector;
            }    
        }
    }

    public Vector2 GetLookVector()
    {
        if (_currentInputType == InputType.Controller)
        {
            if (Mathf.Abs(_joypadAxis.X) > _joypadDeadzone || Mathf.Abs(_joypadAxis.Y) > _joypadDeadzone)
            {
                Vector2 lookVector = _lookRotation;
                lookVector.Y -= _joypadAxis.X * _joypadSensitivity;
                lookVector.X -= _joypadAxis.Y * _joypadSensitivity;
                lookVector.X = Mathf.Clamp(lookVector.X, _minLookAngle, _maxLookAngle);
                _lookRotation = lookVector;
            }
        }
        
        return _lookRotation;
    }
}