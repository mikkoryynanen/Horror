using Godot;
using System;
using System.Collections.Generic;

public partial class Room : Node3D
{
	private MeshInstance3D _northFace;
	private MeshInstance3D _southFace;
	private MeshInstance3D _westFace;
	private MeshInstance3D _eastFace;

	public override void _Ready()
	{
		_northFace = GetNode<MeshInstance3D>("North");
		_southFace = GetNode<MeshInstance3D>("South");
		_eastFace = GetNode<MeshInstance3D>("East");
		_westFace = GetNode<MeshInstance3D>("West");
	}

	public void UpdateFaces(Vector2I myGridPosition, ICollection<Vector2I> gridPositions)
	{
		if (gridPositions.Contains(myGridPosition + Vector2I.Down))
			_northFace.QueueFree();
		// if (gridPositions.Contains(myGridPosition + Vector2I.Up))
		// 	_northFace.QueueFree();
		// if (gridPositions.Contains(myGridPosition + Vector2I.Left))
		// 	_eastFace.QueueFree();
		// if (gridPositions.Contains(myGridPosition + Vector2I.Right))
		// 	_westFace.QueueFree();
	}
}