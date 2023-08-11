using Godot;

namespace Horror.Scripts.Autoload;

public partial class InputManager : Node
{
    public static InputManager Instance => _instance;
    private static InputManager _instance;

    private bool _canProress = true;

    private Vector2 _lookRotation;
    private Vector2 _moveVector;

    private float _minLookAngle = -80;
    private float _maxLookAngle = 90;

    private Vector2 _keyboardInputVector;
    private float _mouseSensitivity = 0.2f;
    private Vector2 _mouseRelative;

    private float _joypadSensitivity = 0.5f;
    private float _joypadDeadzone = 0.3f;
    private Vector2 _joypadRightAxisRaw;
    private Vector2 _joypadLeftAxisRaw;
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
        if (!_canProress)
        {
            return;
        }
        
        if (_currentInputType == InputType.Controller)
        {
            _joypadLeftAxisRaw = new (Input.GetJoyAxis(0, JoyAxis.LeftX), Input.GetJoyAxis(0, JoyAxis.LeftY));
            _joypadRightAxisRaw = new (Input.GetJoyAxis(0, JoyAxis.RightX), Input.GetJoyAxis(0, JoyAxis.RightY));
        }
        else
        {
            _keyboardInputVector = Input.GetVector("left", "right", "forward", "back");
        }

        if (Input.IsActionJustPressed("cancel"))
        {
            this.EmitSignalBus("OnMenuCancel");
        }
    }
    
    public override void _Input(InputEvent inputEvent)
    {
        if (!_canProress)
        {
            return;
        }

        
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

    /// <summary>
    /// Calculated left joystick input with deadzone
    /// </summary>
    public Vector2 GetMoveVector()
    {
        if (_currentInputType == InputType.Controller)
        {
            _joypadLeftAxisRaw = new Vector2(
                Mathf.Abs(_joypadLeftAxisRaw.X) < _joypadDeadzone ? 0 : _joypadLeftAxisRaw.X,
                Mathf.Abs(_joypadLeftAxisRaw.Y) < _joypadDeadzone ? 0 : _joypadLeftAxisRaw.Y);
            return _joypadLeftAxisRaw;
        }
        else
        {
            return _keyboardInputVector;
        }
    }

    public Vector2 GetLookVector()
    {
        if (_currentInputType == InputType.Controller)
        {
            if (Mathf.Abs(_joypadRightAxisRaw.X) > _joypadDeadzone || 
                Mathf.Abs(_joypadRightAxisRaw.Y) > _joypadDeadzone)
            {
                Vector2 lookVector = _lookRotation;
                lookVector.Y -= _joypadRightAxisRaw.X * _joypadSensitivity;
                lookVector.X -= _joypadRightAxisRaw.Y * _joypadSensitivity;
                lookVector.X = Mathf.Clamp(lookVector.X, _minLookAngle, _maxLookAngle);
                _lookRotation = lookVector;
            }
        }
        
        return _lookRotation;
    }

    public void SetProcessState(bool state)
    {
        _canProress = state;
    }
}