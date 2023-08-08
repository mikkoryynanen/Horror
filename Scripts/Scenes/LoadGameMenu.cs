using Godot;

namespace Horror.Scripts.Scenes;

public partial class LoadGameMenu : Control
{
	public override void _Ready()
	{
		GetNode<Button>("Button").ButtonDown += () => MenuManager.Instance.ShowMenu(MenuManager.Menu.Main);
	}
}