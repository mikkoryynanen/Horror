using Godot;

namespace Horror.Scripts.Player;

public partial class ViewObjectManager : Node
{
	private Camera3D _objectCamera;


	public override void _Ready()
	{
		_objectCamera = GetNode<Camera3D>("SubViewport/ObjectCamera");
	}

	public void UpdateCameraTransform(Transform3D transform)
	{
		_objectCamera.GlobalTransform = transform;
	}
}