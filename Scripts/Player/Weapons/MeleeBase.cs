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
        
        if (Input.IsActionJustPressed("interact"))
        {
            Animator.Play("PlayerArmsAnimationLibrary/Interact");
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
                    damageable.TakeDamage(25);
                    hitDamageable = true;
                    
                    var packed = ResourceLoader.Load<PackedScene>("res://Prefabs/Effects/BloodEffect.tscn");
                    var instance = packed.Instantiate() as Node3D;
                    GetNode<Node3D>("/root/Root").AddChild(instance);
                    
                    instance.Position = new Vector3(body.Position.X, body.Position.Y + 1f, body.Position.Z);
                    
                }
            }
        }
        
        AnimationTree.Set("parameters/shoot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
        
        PlayAudio(hitDamageable);
    }

    public override bool CanShoot()
    {
        return !AnimationTree.Get("parameters/shoot/active").AsBool();
    }

    private void PlayAudio(bool hitDamageable)
    {
        AudioManager.Instance.PlayClip(hitDamageable
            ? AudioManager.AudioClipName.MeleeHitBody
            : AudioManager.AudioClipName.Melee);
    }
}