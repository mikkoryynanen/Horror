using System;
using Godot;
using Array = Godot.Collections.Array;

namespace Horror.Scripts;

public static class Loader
{
    public delegate void LoadingEvent(string path);
    public static event LoadingEvent OnLoadingCompleted;
    
    /// <summary>
    /// Loads resource with given path
    /// </summary>
    public static Error Load(string path)
    {
        if (!ResourceLoader.Exists(path))
        {
            GD.PrintErr($"Path {path} does not exist");
        }
        else
        {
            return ResourceLoader.LoadThreadedRequest(path, "", true, ResourceLoader.CacheMode.Reuse);
        }

        return Error.Unavailable;
    }

    /// <summary>
    /// Gets the current loading progress of the given resource
    /// </summary>
    public static Variant GetLoadingProgress(string path)
    {
        var progress = new Array();
        var status = ResourceLoader.LoadThreadedGetStatus(path, progress);
        switch (status)
        {
            case ResourceLoader.ThreadLoadStatus.InvalidResource:
                GD.PrintErr("Scene Loading failed. Invalid resource");
                break;
            case ResourceLoader.ThreadLoadStatus.InProgress:
                // GD.Print($"Loading... {progress[0]}");
                return progress[0];
            case ResourceLoader.ThreadLoadStatus.Failed:
                GD.PrintErr("Scene Loading failed");
                break;
            case ResourceLoader.ThreadLoadStatus.Loaded:
                GD.Print("Scene loaded");
                OnLoadingCompleted?.Invoke(path);
                break;
        }

        return 1f;
    }

    /// <summary>
    /// Use this to get the loaded resource
    /// </summary>
    public static PackedScene GetLoadedScene(string path)
    {
        return ResourceLoader.LoadThreadedGet(path) as PackedScene;
    }

    public static T Instantiate<T>(string path) where T: class
    {
        var packed = ResourceLoader.Load<PackedScene>(path);
        return packed.Instantiate() as T;
    }
}