using System;
using System.Threading.Tasks;
using Godot;

namespace Horror.Scripts.Autoload;

public enum AudioClipName
{
    Footsteps,
    Melee,
    MeleeHit
}

/// <summary>
/// Wrapper for FMOD GDScript methods. More info at https://alessandrofama.com/tutorials/fmod/godot/
/// </summary>
public partial class AudioManager : Node
{
    public static AudioManager Instance => _instance;
    private static AudioManager _instance;
    
    private GodotObject _audioPlayerObject;
    
    private string _currentIntensity = "Basic";

    public enum AudioClipName
    {
        Footsteps,
        Melee,
        MeleeHit,
        MeleeHitBody,
        Breathe,
        
        UIClick,
        UIConfirm,
        
        MeleePickup,
        
        PistolShoot,
        PistolReload
    }
    
    
    public override void _EnterTree(){
        if(_instance != null){
            this.QueueFree();
        }
        _instance = this;
    }

    public override void _Ready()
    {
        _audioPlayerObject = GetNode<GodotObject>("/root/Root/AudioPlayer");
    }

    public void PlayLevelMusic()
    {
        _audioPlayerObject.Call("play_music");
    }

    public void ChangeIntensity()
    {
        _currentIntensity = _currentIntensity == "High" ? "Basic" : "High";
        _audioPlayerObject.Call("on_set_intensity", _currentIntensity);
    }
    
    /// <summary>
    /// Ends the running and sets the exhaustion level
    /// </summary>
    /// <param name="exhaustionLevel">Low or High</param>
    public void SetBreatheEnd(string exhaustionLevel)
    {
        _audioPlayerObject.Call("on_set_breathe_end", exhaustionLevel);
    }

    public void PlayClip(AudioClipName clipName)
    {
        switch (clipName)
        {
            case AudioClipName.Footsteps:
                _audioPlayerObject.Call("on_footstep");
                break;
            case AudioClipName.Melee:
            {
                _audioPlayerObject.Connect(
                    "on_audio_played", 
                    Callable.From(OnAudioPlayed), 
                    (uint)ConnectFlags.OneShot);
                
                _audioPlayerObject.Call("on_melee");
            }
                break;
            case AudioClipName.MeleeHit:
            {
                _audioPlayerObject.Connect(
                    "on_audio_played", 
                    Callable.From(OnAudioPlayed), 
                    (uint)ConnectFlags.OneShot);
                
                _audioPlayerObject.Call("on_melee_hit");
            }
                break;
            case AudioClipName.MeleeHitBody:
            {
                _audioPlayerObject.Call("on_melee_hit_body");
            }
                break;
            case AudioClipName.Breathe:
            {
                _audioPlayerObject.Call("on_breathe");
            }
                break;
            case AudioClipName.UIClick:
            {
                _audioPlayerObject.Call("on_ui_click");
            }
                break;
            case AudioClipName.UIConfirm:
            {
                _audioPlayerObject.Call("on_ui_confirm");
            }
                break;
            case AudioClipName.MeleePickup:
            {
                _audioPlayerObject.Call("on_melee_pickup");
            }
                break;
            case AudioClipName.PistolShoot:
            {
                _audioPlayerObject.Call("on_pistol_shoot");
            }
                break;
            case AudioClipName.PistolReload:
            {
                _audioPlayerObject.Call("on_pistol_reload");
            }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(clipName), clipName, null);
        }
    }

    private void OnAudioPlayed()
    {
    }
}