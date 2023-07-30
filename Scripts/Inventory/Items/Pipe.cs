namespace Horror.Scripts.Inventory.Items;

public class Pipe : Item
{
    public Pipe() 
        : base(
            "Pipe", 
            "res://Prefabs/Items/Pickupable/Pipe.tscn", 
            "res://Assets/UI/Icons/Pipe.png", 
            "Very basic piece of pipe. As sturdy as they come")
    {
    }
}