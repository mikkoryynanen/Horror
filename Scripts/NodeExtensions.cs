using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Horror.Scripts.Autoload;

namespace Horror.Scripts;

public static class NodeExtensions
{
    public static SignalBus GetSignalBus(this Node node)
    {
        return node.GetNode<SignalBus>("/root/SignalBus");
    }

    public static void EmitSignalBus(this Node node, string signalName, params Variant[] args)
    {
        GetSignalBus(node).EmitSignal(signalName, args);
    }

    public static Godot.Collections.Dictionary GetCameraCenterHitObjects3D(this Node3D node)
    {
        var viewport = node.GetViewport();
        var camera = viewport.GetCamera3D();
        
        var screenPoint = new Vector2I((int)viewport.GetVisibleRect().Size.X / 2, (int)viewport.GetVisibleRect().Size.Y / 2);
        var from = camera.ProjectRayOrigin(screenPoint);
        var to = from + camera.ProjectRayNormal(screenPoint) * 100;
		
        var spaceState = node.GetWorld3D().DirectSpaceState;
        var query = PhysicsRayQueryParameters3D.Create(from, to);
        return spaceState.IntersectRay(query);
    }
    
    public static TKey FindKeyByValue<TKey, TValue>(this System.Collections.Generic.Dictionary<TKey, IReadOnlyList<TValue>> dictionary, TValue targetValue)
    {
        foreach (var kvp in dictionary.Where(kvp => kvp.Value.Contains(targetValue)))
        {
            return kvp.Key;
        }

        return default;
    }
}