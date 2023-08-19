using Godot;

namespace Horror.Scripts.Scenes;

public partial class MenuManager : Node
{
	public static MenuManager Instance => _instance;
	private static MenuManager _instance;

	public enum Menu
	{
		Main,
		LoadGame,
		Options,
		Loading
	}
	private Godot.Collections.Dictionary<Menu, string> _menuPaths = new()
	{
		{ Menu.Main, "res://Scenes/Menus/MainMenu.tscn"},
		{ Menu.Options, "res://Scenes/Menus/OptionsMenu.tscn" },
		{ Menu.LoadGame, "res://Scenes/Menus/LoadGameMenu.tscn" },
		{ Menu.Loading, "res://Scenes/Menus/Loading.tscn" },
	};

	// TODO Currently menus are loaded in _menus tho
	private Godot.Collections.Dictionary<Menu, Node> _loadedMenus = new();
	private Node _currentMenu;
	private Error _sceneLoaeder;
	

	public override void _EnterTree()
	{
		if (_instance == null)
			_instance = this;
	}
	
	/// <summary>
	/// Used to show menu. Does not unload previous menus, unless specified
	/// </summary>
	public Node ShowMenu(Menu menuName)
	{
		HidePreviousMenu();
		
		if (_loadedMenus.TryGetValue(menuName, out var loadedMenu))
		{			
			if (loadedMenu is Control control)
				control.Visible = true;
			
			_currentMenu = loadedMenu;
			
			return null;
		}
		
		if (_menuPaths.TryGetValue(menuName, out var path))
		{
			var menu = GD.Load<PackedScene>(path).Instantiate();

			GetTree().Root.CallDeferred("add_child", menu);			
			_currentMenu = menu;
			
			_loadedMenus.Add(menuName, menu);

			return menu;
		}

		return null;
	}

	public void UnloadMenu(Menu menu)
	{
		if (_loadedMenus.TryGetValue(menu, out var loadedMenu))
		{
			loadedMenu.QueueFree();
		}
		_loadedMenus.Remove(menu);
	}

	public void UnloadAllMenus()
	{
		foreach (var loadedMenu in _loadedMenus.Values)
			loadedMenu.QueueFree();
	}

	/// <summary>
	/// Used to completely change scenes, unloading all of the previous ones in the process
	/// </summary>
	public Node AddScene(PackedScene scene, Node parent = null)
	{
		var instance = scene.Instantiate() as Node;
		
		if (parent == null)
			GetTree().Root.AddChild(instance);
		else
			parent.AddChild(instance);

		return instance;
	}
	
	// Helpers =========================================================================
	public void StartGame()
	{
		// Open loading menu and pass level path to it
		// TODO We could get this from game saves
		var menu = ShowMenu(Menu.Loading);
		if (menu is Loading loadingMenu)
			// TODO Using first act for now, but give it save games level
			loadingMenu.Start("res://Prefabs/Rooms/Act0/Room0/Act_0_Room_0.tscn", false);
			
		// ChangeScene("");
	}
	// =================================================================================
	
	private void HidePreviousMenu()
	{
		if (_currentMenu is Control control)
			control.Visible = false;
	}
}