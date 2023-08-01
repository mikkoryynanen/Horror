using System;
using Godot;

namespace Horror.Scripts.Player.Weapons;

/// <summary>
/// Base class for all weapon types
/// </summary>
public partial class WeaponBase : Node3D, IWeapon
{
    protected AnimationPlayer Animator;
    
    public override void _Ready()
    {
        Animator = GetNode<AnimationPlayer>("AnimationPlayer");
        
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
        return Animator.CurrentAnimation == "idle";
    }
}