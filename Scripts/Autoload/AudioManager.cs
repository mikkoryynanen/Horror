using Godot;

namespace Horror.Scripts.Autoload;

public partial class AudioManager : Node
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;
 
    private AudioStreamPlayer _player;
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

    public void PlayClip(AudioStream stream)
    {
        if (!_player.Playing)
        {
            _player.Stream = stream;
            _player.Play();    
        }
    }

    public bool HasPlayedClip()
    {
        return !_player.Playing;
    }
}