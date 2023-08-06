using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Player.Weapons;

/// <summary>
/// Base class for firearms. Do not instantiate on it's own or attach to a node
/// </summary>
public partial class FirearmBase : WeaponBase
{
    private float _fireRate = 0.5f;
    private int _clipSize = 7;
    
    private Camera3D _camera;
    
    protected bool IsAutomatic = true;
    protected int CurrentAmmo = 45;
    protected int TotalAmmo = 14;
    
    private float _pressedDownTimer;
    private bool _isPressingDown;

    public override void _Ready()
    {
        base._Ready();

        _camera = GetViewport().GetCamera3D();
        CurrentAmmo = 7;
        
        this.EmitSignalBus("OnUpdateAmmo", CurrentAmmo, TotalAmmo);
    }

    public override void _Process(double delta)
    {
        if (CanFire() && Input.IsActionJustPressed("shoot"))
        {
            _isPressingDown = true;
            Shoot();
            PlayAudio();
        }
		
        if (Input.IsActionJustReleased("shoot"))
        {
            _pressedDownTimer = 0;
            _isPressingDown = false;
        }
        
        if (IsAutomatic && _isPressingDown && CurrentAmmo > 0)
        {
            _pressedDownTimer += (float)delta;
            if (_pressedDownTimer >= _fireRate)
            {
                _pressedDownTimer = 0;
                
                Shoot();
                PlayAudio();
            }
        }
        
        if (TotalAmmo > 0 && Input.IsActionJustPressed("reload"))
        {
            Reload();
        }
    }
    
     private bool CanFire()
     {
         return CurrentAmmo > 0 &&
                !AnimationTree.Get("parameters/reload/active").AsBool() &&
                !AnimationTree.Get("parameters/shoot/active").AsBool();
     }

    public override void Shoot()
    {        
        CurrentAmmo--;
        
        var result = this.GetCameraCenterHitObjects3D();
        if (result.Count > 0)
        {
            if (result["collider"].AsGodotObject() is IDamageable damageable)
            {
                damageable.TakeDamage(25);
            }
            
            var packedEffect = ResourceLoader.Load<PackedScene>("res://Prefabs/Particles/HitParticle.tscn");
            var node = packedEffect.Instantiate() as Node3D;
            GetNode<Node3D>("/root/Root").AddChild(node);
            node.Position = result["position"].AsVector3();
        }
        
        AnimationTree.Set("parameters/shoot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        
        this.EmitSignalBus("OnUpdateAmmo", CurrentAmmo, TotalAmmo);
        this.EmitSignalBus(nameof(SignalBus.OnCameraShake), 0.25f);
    }

    private void Reload()
    {
        var ammoDelta = _clipSize - CurrentAmmo;
        var ammoToReload = ammoDelta;
        
        if (TotalAmmo >= ammoDelta)
            TotalAmmo -= ammoDelta;
        else
        {
            ammoToReload = TotalAmmo;
            TotalAmmo = 0;
        }
        
        CurrentAmmo += ammoToReload;
        
        this.EmitSignalBus("OnUpdateAmmo", CurrentAmmo, TotalAmmo);
        AnimationTree.Set("parameters/reload/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.PistolReload);
    }

    private void PlayAudio()
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.PistolShoot);
    }
}