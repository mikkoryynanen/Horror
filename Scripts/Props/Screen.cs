using Godot;
using System;

public partial class Screen : Node3D
{
	// [Export()] private VideoStream _videoStream;
	private VideoStreamPlayer _videoPlayer;

	public override void _Ready()
	{
		_videoPlayer = GetNode<VideoStreamPlayer>("SubViewport/VideoStreamPlayer");
		
		// _videoPlayer.Stream = _videoStream;
		// _videoPlayer.Autoplay = true;
		_videoPlayer.Finished += () => _videoPlayer.Play();
	}

	// public override void _Process(double delta)
	// {
	// 	if (_videoStream == null)
	// 	{
	// 		return;
	// 	}
	// 	
	// 	if (!_videoPlayer.IsPlaying())
	// 	{
	// 		_videoPlayer.Play();
	// 	}
	// }
}
