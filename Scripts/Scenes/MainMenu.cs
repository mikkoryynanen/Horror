using Godot;

namespace Horror.Scripts.Scenes;

public partial class MainMenu : Control
{
	public override void _Ready()
	{
		var continueButton = GetNode<Button>("Margin/Box/VBox/Continue");
		var loadButton = GetNode<Button>("Margin/Box/VBox/Load");
		var optionsButton = GetNode<Button>("Margin/Box/VBox/Options");
		var quitButton = GetNode<Button>("Margin/Box/VBox/Quit");

		// continueButton.Disabled = !HasSavedGames();
		continueButton.ButtonDown += () =>
		{
			MenuManager.Instance.StartGame();
		};
		
		optionsButton.ButtonDown += () => MenuManager.Instance.ShowMenu(MenuManager.Menu.Options);
		loadButton.ButtonDown += () => MenuManager.Instance.ShowMenu(MenuManager.Menu.LoadGame);
		quitButton.ButtonDown += () => GD.Print("QUIT game");
	}

	private bool HasSavedGames()
	{
		return false;
	}
}