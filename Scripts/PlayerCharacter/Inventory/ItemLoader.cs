
using System.Collections.Generic;
using System.Text.Json;
using Godot;

public static class ItemLoader
{
    public class ItemDb
    {
        public int ID { get; set; }
        public string name { get; set; }
        public string texturePath { get; set; }
    }

    public class ItemDatabase
    {
        public List<ItemDb> Items { get; set; }
    }

    public static Godot.Collections.Dictionary<int, ItemData> LoadItemData(string ItemDataPath)
    {
        var file = FileAccess.Open(ItemDataPath, FileAccess.ModeFlags.Read);
        if (file == null)
        {
            Logger.Fatal("Failed to open item database file with path {0}", ItemDataPath);
        }
        var data = file.GetAsText();
        if (data == "") {
            Logger.Fatal("Failed to read item data from json with path {0}", ItemDataPath);
            return null;
        }
        ItemDatabase dataDb = JsonSerializer.Deserialize<ItemDatabase>(data);
        if (dataDb == null)
        {
            Logger.Fatal("Failed to parse item data from json with path {0}", ItemDataPath);
            return null;
        }
        Logger.Info("Deserialized item Database");
        return GenerateItems(dataDb);
    }
    
    private static Godot.Collections.Dictionary<int, ItemData> GenerateItems(ItemDatabase itemDb)
    {
        var itemList = new Godot.Collections.Dictionary<int, ItemData>();

        foreach (var dbItem in itemDb.Items)
        {
            var item = new ItemData();
            item.Id = dbItem.ID;
            item.Name = dbItem.name;
            item.TexturePath = dbItem.texturePath;

            itemList[item.Id] = item;
        }
        return itemList;
    }
}