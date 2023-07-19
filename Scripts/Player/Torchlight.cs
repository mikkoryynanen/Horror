using Godot;
using Horror.Scripts;

public partial class Torchlight : SpotLight3D
{
    private double _energy = 50;
    private double _maxEnergy = 100;
    
    private float _defaultLightEnergy;
    private float _defaultSpotAngle;
    private float _timescale = 1f;

    public override void _Ready()
    {
        _defaultLightEnergy = LightEnergy;
        _defaultSpotAngle = SpotAngle;
        
        this.GetSignalBus().OnRechargeTorchlight += OnRecharge;
        this.GetSignalBus().OnChangeTimescale += timescale => _timescale = timescale;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_energy <= 0)
        {
            LightEnergy = 0;
        }
        else
        {
            _energy -= delta * _timescale;
            UpdateSpotlight();
        }
    }
    
    private void OnRecharge(float value)
    {
        _energy += value;
        UpdateSpotlight();
    }

    private void UpdateSpotlight()
    {
        _energy = Mathf.Clamp(_energy, 0, _maxEnergy);
        
        var energyNormal = (float)(_energy / _maxEnergy);
        
        LightEnergy = energyNormal;
        
        SpotAngle = energyNormal * 100;
        SpotAngle = Mathf.Clamp(SpotAngle, 25, 45);
    }

    private void Reset()
    {
        SpotAngle = _defaultSpotAngle;
        LightEnergy = _defaultLightEnergy;
    }
}