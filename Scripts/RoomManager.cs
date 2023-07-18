using Godot;
using System;
using Godot.Collections;

public partial class RoomManager : Node3D
{ 
	[Export()] private Vector2I _mapSize = new Vector2I(10, 10);
	private Dictionary<Vector2I, Room> _rooms = new();


	public override void _Ready()
	{
		GenerateMap();
	}

	private void GenerateMap()
	{
		// var packed = ResourceLoader.Load($"res://Prefabs/room.tscn") as PackedScene;
		//
		// for (int x = 0; x < _mapSize.X; x++)
		// {
		// 	for (int y = 0; y < _mapSize.Y; y++)
		// 	{
		// 		var roomInstance = packed.Instantiate() as Room;
		// 		AddChild(roomInstance);
		// 		
		// 		roomInstance.Position = new Vector3(x * 3, 0, y * 3);
		// 		_rooms.Add(new Vector2I(x, y), roomInstance);
		// 	}
		// }
		//
		// foreach (var room in _rooms)
		// {
		// 	room.Value.UpdateFaces(room.Key, _rooms.Keys);
		// }
	}
}
