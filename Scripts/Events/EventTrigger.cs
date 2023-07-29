using Godot;
using System;

public partial class EventTrigger : Area3D
{
	[Export()] private string _triggerGroupName = "player";

	[Signal]
	public delegate void OnTriggerEventHandler();
	
	public void OnEntered(Node3D node)
	{
		if (node.IsInGroup(_triggerGroupName))
		{
			EmitSignal("OnTrigger");
		}
	}
}
