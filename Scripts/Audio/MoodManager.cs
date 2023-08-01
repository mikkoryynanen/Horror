using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Audio;

public partial class MoodManager : Node
{
    private float _currentMood = 0f;
    private float _maxMood = 100f;
    private float _moodMultiplier = 0;

    private AudioStream[] _moodStreams = new[]
    {
        GD.Load<AudioStream>("res://Assets/sfx/Cinematic/cinematic_0.wav"),
        GD.Load<AudioStream>("res://Assets/sfx/Cinematic/cinematic_1.wav"),
        GD.Load<AudioStream>("res://Assets/sfx/Cinematic/cinematic_2.wav"),
    };

    public override void _Ready()
    {
        this.GetSignalBus().OnMoodIncrease += () => _moodMultiplier = -1f;
        this.GetSignalBus().OnMoodDecrease += () => _moodMultiplier = 1f;

        _currentMood = 0;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.GetProcessFrames() % 2 == 0)
        {
            _currentMood = Mathf.Clamp(_currentMood, 0, _maxMood);
            
            if(_currentMood >= 0)
                _currentMood -= _moodMultiplier * (float)delta;
            
            if (_currentMood >= _maxMood)
            {
                // AudioManager.Instance.PlayClip(_moodStreams[GD.RandRange(0, _moodStreams.Length - 1)]);
                _currentMood = 0;
            }
        }
    }
}