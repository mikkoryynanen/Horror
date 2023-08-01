using System;
using System.IO;
using Godot;

namespace Horror.Scripts.Inventory;

public abstract class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string PrefabPath { get; set; }
    public string IconPath { get; set; }

    public enum Type
    {
        Item,
        Weapon
    }

    public Type ItemType { get; set; }
    
    protected Item(string name, string prefabPath, string iconPath, string description, Type type)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        PrefabPath = prefabPath;
        IconPath = iconPath;
        ItemType = type;
        
        GD.Print($"Item {Name} of type {ItemType} created with id {Id}");
    }
    
    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(Id.ToString());
        writer.Write(Name);
        writer.Write(Description);
        writer.Write(PrefabPath);
        writer.Write(IconPath);
        writer.Write((int)ItemType);
    }

    public virtual void Deserialize(BinaryReader reader)
    {
        Id = new Guid(reader.ReadString());
        Name = reader.ReadString();
        Description = reader.ReadString();
        PrefabPath = reader.ReadString();
        IconPath = reader.ReadString();
        ItemType = (Type)reader.ReadInt32();
    }
}