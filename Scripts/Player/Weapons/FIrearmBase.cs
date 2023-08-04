using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Player.Weapons;

/// <summary>
/// Base class for firearms. Do not instantiate on it's own or attach to a node
/// </summary>
public partial class FirearmBase : WeaponBase
{
    [Export()] private float _fireRate = 0.5f;
    [Export()] private int _clipSize = 7;
    
    private Camera3D _camera;
    
    protected bool IsAutomatic = true;
    protected int CurrentAmmo = 45;
    protected int TotalAmmo = 14;
    
    private float _pressedDownTimer;
    private bool _isPressingDown;
    private bool _shootAnimFinished = true;

    public override void _Ready()
    {
        base._Ready();

        _camera = GetViewport().GetCamera3D();
        CurrentAmmo = _clipSize;
        
        Animator.AnimationFinished += OnAnimationFinished;
        
        this.EmitSignalBus("OnUpdateAmmo", CurrentAmmo, TotalAmmo);
    }
    
    private void OnAnimationFinished(StringName animName)
    {
        if (animName == "shoot")
        {
            _shootAnimFinished = true;
        }
    }

    public override void _Process(double delta)
    {
        if (CanFire() && Input.IsActionJustPressed("shoot") )
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
        return CurrentAmmo > 0;
    }

    public override void Shoot()
    {
        var results = this.GetCameraCenterHitObjects3D();
        if (results.Count > 0)
        {
            var packedEffect = ResourceLoader.Load<PackedScene>("res://Prefabs/Particles/HitParticle.tscn");
            var node = packedEffect.Instantiate() as Node3D;
            GetNode<Node3D>("/root/Root").AddChild(node);
            node.Position = results["position"].AsVector3();
        }
        
        _shootAnimFinished = false;
        CurrentAmmo--;
        
        AnimationTree.Set("parameters/shoot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        this.EmitSignalBus("OnUpdateAmmo", CurrentAmmo, TotalAmmo);
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
        
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.PistolReload);
    }

    private void PlayAudio()
    {
        AudioManager.Instance.PlayClip(AudioManager.AudioClipName.PistolShoot);
    }
}