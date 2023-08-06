using Godot;

namespace Horror.Scripts;

public partial class CameraShaker : Camera3D
{
    [Export()] private float _traumaReductionRate = 1f;
    [Export()] private float _trauma = 0f;
    [Export()] private float _noiseSpeed = 50f;
    [Export()] private Vector3 _maxVector = new Vector3(10, 10, 5);
    [Export()] private Noise _noise;

    private float _time;
    private Vector3 _initialRotation;
    private bool _IsContinious = false;


    public override void _Ready()
    {
        var signalBus = this.GetSignalBus(); 
        signalBus.OnCameraShake += AddTrauma;
        signalBus.OnCameraShakeContinuous += trauma =>
        {
            AddTrauma(trauma);
            _IsContinious = true;
        };
        signalBus.OnCameraShakeStop += () =>
        {
            _trauma = 0;
            _IsContinious = false;
        };
        
        _initialRotation = RotationDegrees;
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        if (!_IsContinious)
        {
            _trauma = Mathf.Max(_trauma - (float)delta * _traumaReductionRate, 0);
        }

        RotationDegrees = new Vector3(
            _initialRotation.X + _maxVector.X * GetShakeIntensity() * GetNoiseFromSeed(1),
            _initialRotation.Y + _maxVector.Y * GetShakeIntensity() * GetNoiseFromSeed(2),
            _initialRotation.Z + _maxVector.Z * GetShakeIntensity() * GetNoiseFromSeed(3));
    }

    private void AddTrauma(float amount)
    {
        if (_trauma == 0)
            _trauma = Mathf.Clamp(_trauma + amount, 0, 1);
    }

    private float GetShakeIntensity()
    {
        return _trauma * _trauma;
    }

    private float GetNoiseFromSeed(int seed)
    {
        _noise.Set("seed", seed);
        return _noise.GetNoise1D(_time * _noiseSpeed);
    }
}