using Godot;
using Horror.Scripts.Autoload;
using Horror.Scripts.Damage;

namespace Horror.Scripts.Player.Weapons;

/// <summary>
/// Base class for melee weapons. Do not instantiate on it's own or attach to a node
/// </summary>
public partial class MeleeBase : WeaponBase
{
    public Area3D MeleeCollisionArea;

    public override void _Ready()
    {
        base._Ready();
        
        this.EmitSignalBus("OnUpdateAmmo", 0, 0);
    }

    public override void _Process(double delta)
    {
        if (MeleeCollisionArea == null)
        {
            GD.PrintErr("Melee collision is not setup");
            return;
        }
        
        if (Input.IsActionJustPressed("shoot") && CanShoot())
        {
            Shoot();
        }
    }

    public override void Shoot()
    {
        // Consume stamina
        this.EmitSignalBus("OnReduceStamina", 0.2f);
        
        // TODO stamina could increase the damage of your weapon

        var overlappingBodies = MeleeCollisionArea.GetOverlappingBodies();
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
        
        PlayAudio(hitDamageable);
    }

    private void PlayAudio(bool hitDamageable)
    {
        AudioManager.Instance.PlayClip(hitDamageable
            ? AudioManager.AudioClipName.MeleeHit
            : AudioManager.AudioClipName.Melee);
    }
}