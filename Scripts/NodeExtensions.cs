using Godot;
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
}