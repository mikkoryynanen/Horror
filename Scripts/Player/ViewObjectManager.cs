using Godot;

namespace Horror.Scripts.Player;

public partial class ViewObjectManager : Node
{
	private Node3D _root;


	public override void _Ready()
	{
		_root = GetNode<Node3D>("SubViewport/Root");
		GetNode<SubViewport>("SubViewport").Size = new Vector2I(
			(int)GetViewport().GetVisibleRect().Size.X,
			(int)GetViewport().GetVisibleRect().Size.Y);

	}

	public void UpdateView(Transform3D transform)
	{
		_root.GlobalTransform = transform;
	}
}