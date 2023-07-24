using System.Collections.Generic;
using Godot;

namespace Horror.Scripts.UI;

public partial class Terminal : Control
{
	// private TextWriter _textWriter;

	public override void _Ready()
	{
		// _textWriter = GetNode<TextWriter>("BackgroundColor/Overlay/Margin/TextLabel");
		// _textWriter.Start("./program.sh\nProgram loaded.\n" +
		//                   "1. List all employees\n" +
		//                   "2. Show map\n" +
		//                   "3. Show messages");
		GetNode<Button>("BackgroundColor/Margin/Panel/VBox/Button").GrabFocus();
	}

	// public override void _Input(InputEvent @event)
	// {
	// 	GetViewport().PushInput(@event);
	// }
}