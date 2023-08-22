using Godot;
using Horror.Scripts.Inventory;
using Horror.Scripts.Systems;

namespace Horror.Scripts.Interactables;

public partial class QuestCompleter : StaticBody3D, IInteractable
{
    private IUnlockable _parentUnlockable;

    public override void _Ready()
    {
        // This assumes that the parent is another interactable that this one controls
        _parentUnlockable = GetParent<IUnlockable>();
        if (_parentUnlockable == null)
        {
            GD.PrintErr($"Quest completer requires parent to extend from IInteractable");
            return;
        }
    }

    public void Interact()
    {
        var completed = QuestSystem.Instance.CompleteQuest("0", new []{ ItemDatabase.GetItem("Quest item") });
        GD.Print($"Quest completed? {completed}");
        
        if (completed)
            _parentUnlockable.Unlock();
    }
}