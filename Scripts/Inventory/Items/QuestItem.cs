namespace Horror.Scripts.Inventory.Items;

public class QuestItem : Item
{
    public QuestItem() 
        : base(
            "Quest item", 
            "res://Prefabs/Items/Pickupable/Pipe.tscn", 
            "res://Assets/UI/Icons/Pipe.png",
            "This is test quest item. This can be used to unlock something...")
    {
    }
}