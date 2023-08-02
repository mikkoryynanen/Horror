using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Horror.Scripts.Inventory.Items;

namespace Horror.Scripts.Inventory;

// TODO This could be named to database?
public static class ItemDatabase
{
    private static readonly Dictionary<Guid, Item> Database = new();

    private const string DatabasePath = "user://ItemDatabase.bin";
    public static string GlobalPath => ProjectSettings.GlobalizePath(DatabasePath);

    static ItemDatabase()
    {
        // =============================================
        // TODO Generate items here for now
        // TODO This generates the same times for all inventories, for example chests
        // AddItem(new Pipe());
        // AddItem(new Pistol());
        // AddItem(new QuestItem());
        // Save();
        // =============================================
        
        Load();
    }
    
    public static Item GetItem(Guid itemId)
    {
        return Database.TryGetValue(itemId, out var item) ? item : null;
    }
    
    // TODO Remove?
    public static Item GetItem(string itemName)
    {
        // Make sure we haven't passed guid as item name
        if (Guid.TryParse(itemName, out var guid))
        {
            GD.PrintErr("Passed Guid as item name to ItemDatabase.GetItem");
            return null;
        }
        
        return Database.Values.FirstOrDefault(x => x.Name.ToLower() == itemName.ToLower());
    }
    
    /// <summary>
    /// Gets all of the items in the database
    /// </summary>
    public static IReadOnlyList<Item> GetItems()
    {
        return Database.Values.ToList();
    }

    // public static bool RemoveItem(Guid itemId)
    // {
    //     return Database.Remove(itemId);
    // }

    public static void Load()
    {
        if (!File.Exists(GlobalPath))
            return;

        using var reader = new BinaryReader(File.Open(GlobalPath, FileMode.Open));
        var count = reader.ReadInt32();

        for (int i = 0; i < count; i++)
        {
            var typeName = reader.ReadString();
            var objectType = Type.GetType(typeName);
            if (objectType != null && typeof(Item).IsAssignableFrom(objectType))
            {
                var item = (Item)Activator.CreateInstance(objectType);
                item.Deserialize(reader);
            
                AddItem(item);
            }
        }
    }

    public static void Save()
    {
        using BinaryWriter writer = new(File.Open(GlobalPath, FileMode.Create));
            
        writer.Write(Database.Values.Count);
        foreach (var item in Database.Values)
        {
            writer.Write(item.GetType().AssemblyQualifiedName);
            item.Serialize(writer);
        }
    }
    
    private static void AddItem(Item item)
    {
        try
        {
            Database.TryAdd(item.Id, item);
        }
        catch (Exception e)
        {
            GD.PrintErr(e.Message);
        }
    }
}
