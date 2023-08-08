using Godot;
using System;
using Horror.Scripts.Scenes;

public partial class OptionsMenu : Control
{
	public override void _Ready()
	{
		GetNode<Button>("Margin/BoxContainer/HBoxContainer/Button").ButtonDown += () => MenuManager.Instance.ShowMenu(MenuManager.Menu.Main);
	}
}
