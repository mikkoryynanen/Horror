using Godot;

namespace Horror.Scripts.Utils;

public partial class MaterialUVAnimator : Node3D
{
	[Export()] private int _materialSurface = 1;
	[Export()] private float _changeTime = 1f;
	[Export()] private Vector2[] _offsets;
	
	private Material _material;
	private float _changeTimer = 0f;
	private int _currentOffsetIndex = 0;
	private float _startDelay = 0f;

	public override void _Ready()
	{
		var meshInstance = GetChild<MeshInstance3D>(0);
		if (meshInstance == null)
		{
			GD.PrintErr($"Please add mesh instance as child of {this.Name}");
			return;
		}
		_material = meshInstance.GetActiveMaterial(_materialSurface);

		_startDelay = (float)GD.RandRange(0.0, 3.0);
		GD.Print($"start delay {_startDelay}");
	}
	
	public override void _Process(double delta)
	{
		if (_offsets == null)
		{
			GD.PrintErr($"MaterialUVAnimator does not have Offsets set on object {this.Name}");
			return;
		}
		
		_changeTimer += (float)delta;

		if (_changeTimer < _startDelay)
		{
			return;
		}
		
		if (_changeTimer >= _startDelay + _changeTime)
		{
			var offset = GetCurrentOffset();
			_material.Set("uv1_offset", offset);
			_changeTimer = _startDelay;
		}
	}

	private Vector2 GetCurrentOffset()
	{
		if (_currentOffsetIndex >= _offsets.Length - 1)
			_currentOffsetIndex = 0;
		else
			_currentOffsetIndex++;
		
		return _offsets[_currentOffsetIndex]; 
	}
}