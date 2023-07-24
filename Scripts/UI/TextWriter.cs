using Godot;
using System;

public partial class TextWriter : RichTextLabel
{
	[Export()] private Vector2 _speedRange;

	private float _timer = 0f;
	private string _displayText;
	private float _speedRandom;
	private int _charactersShown;
	
	public override void _Process(double delta)
	{
		if (_charactersShown >= _displayText.Length)
			return;
		
		if (_timer < _speedRandom)
		{
			_timer += (float)delta;
		}
		else
		{
			VisibleCharacters++;
			_charactersShown++;
			_timer = 0;
			_speedRandom = GetRandomSpeed();
		}
	}

	public void Start(string displayText)
	{
		_displayText = displayText;
		Text = _displayText;
		
		VisibleCharacters = 0;
	}

	float GetRandomSpeed()
	{
		return (float)GD.RandRange(_speedRange.X, _speedRange.Y);
	}
}
