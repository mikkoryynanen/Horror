using Godot;
using Horror.Scripts.Autoload;

namespace Horror.Scripts.Props;

public partial class Spotlight : SpotLight3D
{
	public void OnAreaEntered(Node3D node)
	{
		this.EmitSignalBus(nameof(SignalBus.OnMoodDecrease));
	}
	
	public void OnAreaExited(Node3D node)
	{
		this.EmitSignalBus(nameof(SignalBus.OnMoodIncrease));
	}
}