using Godot;

namespace Horror.Scripts;

public partial class GPUParticle : GpuParticles3D
{
    public override void _Ready()
    {
        Emitting = true;
    }

    public override void _Process(double delta)
    {
        if (!Emitting)
            QueueFree();
    }
}