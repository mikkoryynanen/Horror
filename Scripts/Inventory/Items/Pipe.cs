namespace Horror.Scripts.Inventory.Items;

public class Pipe : Item
{
    public Pipe() 
        : base(
            "Pipe", 
            "res://Prefabs/player/Weapons/PipeWeapon.tscn", 
            "res://Assets/UI/Icons/Pipe.png", 
            "Very basic piece of pipe. As sturdy as they come",
            Type.Weapon)
    {
    }
}