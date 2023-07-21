using Godot;

public partial class RoomManager : Node3D
{
	private GridMap _gridmap;

	public override void _Ready()
	{
		_gridmap = GetNode<GridMap>("NavigationRegion/GridMap");
	}

	public Rid GetNavigationMap()
	{
		return _gridmap.GetNavigationMap();
	}
}
