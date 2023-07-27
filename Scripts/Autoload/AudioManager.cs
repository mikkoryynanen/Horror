using System.Threading.Tasks;
using Godot;

namespace Horror.Scripts.Autoload;

public partial class AudioManager : Node
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;
 
    private AudioStreamPlayer _player;
    private AudioStreamPlayer _loopPlayer;
    private AudioStreamPlayer _musicPlayer;

    public override void _EnterTree(){
        if(_instance != null){
            this.QueueFree();
        }
        _instance = this;
    }

    public override void _Ready()
    {
        _player = new AudioStreamPlayer();
        _player.Bus = "SFX";
        AddChild(_player);
        
        _loopPlayer = new AudioStreamPlayer();
        _loopPlayer.Bus = "SFX";
        AddChild(_loopPlayer);
        
        _musicPlayer = new AudioStreamPlayer();
        _musicPlayer.Bus = "Music";
        AddChild(_musicPlayer);
    }

    public void PlayLevelMusic(string path)
    {
        if (!_player.Playing)
        {
            _musicPlayer.Stream = GD.Load<AudioStream>(path);
            _musicPlayer.Play();    
        }
    }

    public void PlayClip(AudioStream stream, float volumeDb = 0f)
    {
        if (!_player.Playing)
        {
            _player.Stream = stream;
            _player.VolumeDb = volumeDb;
            _player.Play();    
        }
    }

    /// <summary>
    /// Adds slight variation to the repeating sound in order to make it more pleasant
    /// </summary>
    /// <param name="stream"></param>
    public void PlayRepeating(AudioStream stream)
    {
        RepeatingLoop(stream);
    }

    private async Task RepeatingLoop(AudioStream stream)
    {
        _loopPlayer.Stream = stream;
        
        while (true)
        {
            
            _loopPlayer.PitchScale = (float)GD.RandRange(0.98, 1.02);
            _loopPlayer.VolumeDb = (float)GD.RandRange(0.9, 1.1);
            _loopPlayer.Play();

            var length = (int)stream.GetLength();
            GD.Print($"Playing repeatable clip. waiting {length}");
            await Task.Delay(length * 1000);
        }
    }

    public bool HasPlayedClip()
    {
        return !_player.Playing;
    }
}