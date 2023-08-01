namespace Horror.Scripts.Inventory.Items;

public class Pistol : Item
{
    public Pistol() 
        : base(
            "Pistol", 
            "res://Prefabs/player/Weapons/PistolWeapon.tscn", 
            "res://Assets/UI/Icons/Pipe.png", 
            "When you need to shoot something",
            Type.Weapon)
    {
    }
}