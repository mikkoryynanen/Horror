using Godot;

namespace Horror.Scripts.Dialog;

public partial class DialogMenu : CanvasLayer
{
    [Export()] public Label characterNameLabel;
    [Export()] public RichTextLabel dialogLabel;
    [Export()] public Node responsesParent;
}