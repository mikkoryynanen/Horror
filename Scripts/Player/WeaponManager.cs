using System;
using Godot;
using Horror.Scripts.Inventory;
using Horror.Scripts.Player.Weapons;

namespace Horror.Scripts.Player;

public partial class WeaponManager : Node3D
{
	[Export()] private float swayIntensity = 0.0005f;

	private float _pressedDownTimer;
	private bool _isPressingDown;
	private bool _canProcess = true;
	private float _t;
	private Area3D _meleeCollisionArea;
	private Node _rig;
	private Node3D _spawnedWeapon;


	public override void _Ready()
	{
		_rig = GetNode("Rig");
		_meleeCollisionArea = GetNode<Area3D>("MeleeCollisionArea");
		
		var signalBus = this.GetSignalBus();
		signalBus.OnStartDialog += (act, title, isFullscreen) =>
		{
			if(isFullscreen)
				_canProcess = false;
		};
		signalBus.OnEndDialog += () => _canProcess = true;
		signalBus.OnItemPickedUp += OnItemPickup;
		signalBus.OnChangeWeapon += OnChangeWeapon;
	}

	public override void _Process(double delta)
	{
		if (!_canProcess) return;
		
		_t = (float)delta * 20f;

		Position = new Vector3(
			Mathf.Lerp(Position.X, 0, _t), 
			Mathf.Lerp(Position.Y, 0, _t),
			Mathf.Lerp(Position.Z, 0, _t));
	}
	
	public void Sway(Vector2 swayAmount)
	{
		Position += new Vector3(swayAmount.X * -swayIntensity, swayAmount.Y * swayIntensity, 0);
	}
	
	private void OnItemPickup(string itemId)
	{
		var item = ItemDatabase.GetItem(new Guid(itemId));
		if (item.ItemType == Item.Type.Weapon)
			SpawnWeapon(item.PrefabPath);
	}

	private void OnChangeWeapon(string weaponId)
	{
		if (_spawnedWeapon != null && !_spawnedWeapon.IsQueuedForDeletion())
		{
			_spawnedWeapon.QueueFree();
			_spawnedWeapon = null;
		}
		
		var item = ItemDatabase.GetItem(new Guid(weaponId));
		SpawnWeapon(item.PrefabPath);
	}

	private void SpawnWeapon(string weaponPrefabPath)
	{
		// Don't spawn weapon if the have already spawned one
		// Rig has melee collision as child, so check more than 1
		if (_spawnedWeapon == null)
		{
			var packed = ResourceLoader.Load<PackedScene>(weaponPrefabPath);
			var instance = packed.Instantiate() as Node3D;
			_spawnedWeapon = instance;
			_rig.AddChild(instance);

			var weapon = instance as IWeapon;
			weapon.TakeOut();

			if (weapon is MeleeBase meleeBase)
				meleeBase.MeleeCollisionArea = _meleeCollisionArea;

		}
		else
			GD.PrintErr("Weapon already spawned. Remove spawned weapon first before spawning a new one");
	}

	private void FireProjectile()
	{
		var packed = ResourceLoader.Load("res://Prefabs/player/flash.tscn") as PackedScene;
		var instance = packed.Instantiate() as Node3D;
		instance.GetNode<AnimationPlayer>("AnimationPlayer").Play("flash");
		GetNode("pistol1/Muzzle").AddChild(instance);
	}
}