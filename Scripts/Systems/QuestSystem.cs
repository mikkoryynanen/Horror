using System.Collections.Generic;
using Horror.Scripts.Inventory;
using Horror.Scripts.Inventory.Items;
using Horror.Scripts.Systems;

namespace Horror.Scripts.Systems;

public class Quest
{
    public string Id { get; set; }
    public IReadOnlyList<Item> RequiredItems { get; set; }
}

public sealed class QuestSystem
{
    public static QuestSystem Instance => instance ??= new QuestSystem();

    private static Dictionary<string, Quest> _quests = new()
    {
        { "0", new Quest { RequiredItems = new []{ ItemDatabase.GetItem("Quest item") }} }
    };

    private static QuestSystem instance = null;

    public Quest GetQuest(string questId)
    {
        return _quests.TryGetValue(questId, out var quest) ? quest : null;
    }

    public bool CompleteQuest(string questId, IReadOnlyList<Item> items)
    {
        var quest = GetQuest(questId);
        foreach (var requiredItem in quest.RequiredItems)
        {
            foreach (var item in items)
            {
                if (item != requiredItem)
                {
                    return false;
                }
            }
        }

        return true;
    }
}