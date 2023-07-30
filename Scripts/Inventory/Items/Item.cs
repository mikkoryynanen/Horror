using System;
using System.IO;

namespace Horror.Scripts.Inventory;

public abstract class Item
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string PrefabPath { get; set; }
    public string IconPath { get; set; }
    
    protected Item(string name, string prefabPath, string iconPath)
    {
        Id = Guid.NewGuid();
        Name = name;
        PrefabPath = prefabPath;
        IconPath = iconPath;
    }
    
    public virtual void Serialize(BinaryWriter writer)
    {
        writer.Write(Id.ToString());
        writer.Write(Name);
        writer.Write(PrefabPath);
        writer.Write(IconPath);
    }

    public virtual void Deserialize(BinaryReader reader)
    {
        Id = new Guid(reader.ReadString());
        Name = reader.ReadString();
        PrefabPath = reader.ReadString();
        IconPath = reader.ReadString();
    }
}