using Godot;
using System;

public partial class InventoryItem : TextureRect
{
	private TextureRect _icon;
	private TextureRect _highlightBorder;

	public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Icon");
		_highlightBorder = GetNode<TextureRect>("HighlightBorder");
	}

	public void Build(Texture2D icon)
	{
		_icon.Texture = icon;
		_icon.Visible = icon != null;

		OnDeselect();
	}

	public void OnSelect()
	{
		_highlightBorder.Visible = true;
	}

	public void OnDeselect()
	{
		_highlightBorder.Visible = false;
	}
}
