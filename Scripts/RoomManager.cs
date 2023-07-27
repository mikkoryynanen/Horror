using Godot;
using Horror.Scripts.Autoload;

public partial class RoomManager : Node3D
{
	private GridMap _gridmap;

	public override void _Ready()
	{
		_gridmap = GetNode<GridMap>("NavigationRegion/GridMap");
		
		AudioManager.Instance.PlayRepeating(GD.Load<AudioStream>("res://Assets/sfx/environment/Sci_Fi_Alarm_Loop_04.wav"));
	}

	public Rid GetNavigationMap()
	{
		return _gridmap.GetNavigationMap();
	}
}
