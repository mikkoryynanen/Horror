using Godot;
using System;
using Horror.Scripts;
using Horror.Scripts.Autoload;
using Horror.Scripts.Damage;

public interface IWeapon
{
    void Shoot();
    void TakeOut();
    void PutAway();
    bool CanShoot();
}

public partial class WeaponBase : Node, IWeapon
{
    public override void _Ready()
    {
        this.GetSignalBus().OnActivatePlayerCamera += TakeOut;
        this.GetSignalBus().OnDeactivatePlayerCamera += PutAway;
    }

    public virtual void Shoot()
    {
        throw new NotImplementedException();
    }

    public virtual void TakeOut()
    {
        throw new NotImplementedException();
    }

    public virtual void PutAway()
    {
        throw new NotImplementedException();
    }

    public virtual bool CanShoot()
    {
        throw new NotImplementedException();
    }
}

// TODO Move to its own class
public partial class MeleeBase : WeaponBase
{
    [Export()] private Area3D _meleeCollisionArea;
    
    public override void _Process(double delta)
    {
        if (_meleeCollisionArea == null)
        {
            GD.PrintErr("Melee collision are is not setup");
            return;
        }
        
        if (Input.IsActionJustPressed("shoot") && CanShoot())
        {
            var overlappingBodies = _meleeCollisionArea.GetOverlappingBodies();
            var hitDamageable = false;
            foreach (var body in overlappingBodies)
            {
                if (body.IsInGroup("enemy"))
                {
                    if (body is IDamageable damageable)
                    {
                        damageable.TakeDamage(10);
                        hitDamageable = true;
						
                        // TODO Enable?
                        // var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/Particles/HitParticle.tscn");
                        // var instance = packed.Instantiate() as Node3D;
                        // instance.Position = _raycast.GetCollisionPoint();
                        // GetNode<Node3D>("/root/Core").AddChild(instance);
                    }
                }
            }
			
            Shoot();
            PlayAudio(hitDamageable);
        }
    }

    public void PlayAudio(bool hitDamageable)
    {
        GetNode<GodotObject>("/root/Root/AudioPlayer").Call(  !hitDamageable ? "on_melee" : "on_melee_hit");
        AudioManager.Instance.PlayClip(hitDamageable
        		? AudioManager.AudioClipName.MeleeHit
        		: AudioManager.AudioClipName.Melee);
    }
}