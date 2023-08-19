using Godot;

namespace Horror.Scripts.Objects;

public partial class LightController : Node3D
{
	private SpotLight3D _regularLight;
	private SpotLight3D _alertLight;
	private MeshInstance3D _bulb;

	public override void _Ready()
	{
		_regularLight = GetNode<SpotLight3D>("RegularLight");
		_alertLight = GetNode<SpotLight3D>("FlashingLight");
		_bulb = GetNode<MeshInstance3D>("Light/Bulb");
		
		StopAlarm();

		this.GetSignalBus().OnShipAlarmStart += OnAlarm;
		this.GetSignalBus().OnShipAlarmStart += StopAlarm;
	}

	private void OnAlarm()
	{
		_regularLight.Visible = false;
		_alertLight.Visible = true;
		_bulb.GetActiveMaterial(0).Set("albedo_color", Colors.Red);
	}

	private void StopAlarm()
	{
		_regularLight.Visible = true;
		_alertLight.Visible = false;
		
		_bulb.GetActiveMaterial(0).Set("albedo_color", Colors.White);
	}
}