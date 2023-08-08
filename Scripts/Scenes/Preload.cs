using Godot;

namespace Horror.Scripts.Scenes;

public partial class Preload : Node
{
	public override void _Ready()
	{
		MenuManager.Instance.ShowMenu(MenuManager.Menu.Main);
	}
}