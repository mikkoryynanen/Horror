using System;
using Godot;

namespace Horror.Scripts.Player.Weapons;

/// <summary>
/// Base class for all weapon types
/// </summary>
public partial class WeaponBase : Node3D, IWeapon
{
    [Obsolete("Do not use this anymore. Start using AnimationTree instead")]
    protected AnimationPlayer Animator;
    
    private AnimationTree _animationTree;
    protected AnimationTree AnimationTree
    {
        get
        {
            if (_animationTree == null)
                _animationTree = GetNode<AnimationTree>("AnimationTree");
            return _animationTree;
        }
    }

    public override void _Ready()
    {
        Animator = GetNode<AnimationPlayer>("AnimationPlayer");
        AnimationTree.Active = true;
        
        this.GetSignalBus().OnActivatePlayerCamera += TakeOut;
        this.GetSignalBus().OnDeactivatePlayerCamera += PutAway;
    }

    public virtual void Shoot()
    {
        throw new NotImplementedException();
    }

    public virtual void TakeOut()
    {
        // throw new NotImplementedException();
    }

    public virtual void PutAway()
    {
        // throw new NotImplementedException();
    }

    public virtual bool CanShoot()
    {
        return true;
    }

    public void SetAnimationTreeForward(float forwardInput, bool isRunning)
    {
        AnimationTree.Set("parameters/Walking/blend_position", !isRunning ? forwardInput : 2);
    }
}