using Godot;
using Godot.Collections;

public partial class PlayerInventory : Node
{
    public Dictionary<int, ItemData> ItemData { get; set; }
    public Dictionary<int, InventoryItem> InventoryItems { get; set; }
    private string itemDatabaseLocation = "res://resources/itemDB.txt";

    public override void _Ready()
    {
        ItemData = ItemLoader.LoadItemData(itemDatabaseLocation);
        if (ItemData == null || ItemData.Count == 0)
        {
            Logger.Fatal("Failed to load item data from database at path {0}", itemDatabaseLocation);
        }

        InventoryItems = new Dictionary<int, InventoryItem>();
        base._Ready();
    }

    public void AddItem(int id, int amount, string reason)
    {
        if (InventoryItems.TryGetValue(id, out InventoryItem item))
        {
            item.Amount += amount;
            Logger.DebugInfo("Added to existing item {0}, new amount {1} with reason {2}", id, item.Amount, reason);
            return;
        }

        if (ItemData[id] == null)
        {
            Logger.Error("Could not find item data of item {0} to add with reason {1}", id, reason);
            return;
        }

        item = new InventoryItem();
        item.Amount = amount;
        InventoryItems[id] = item;
        Logger.DebugInfo("Added item {0} x{1} to inventory with reason {2}", id, amount, reason);
    }

    public void RemoveItem(int id, int amount, string reason)
    {
        if (InventoryItems.TryGetValue(id, out InventoryItem item))
        {
            var leftoverAmount = item.Amount - amount;
            if (leftoverAmount < 0)
            {
                Logger.DebugWarning(
                    "Player does not have enough of item {0} to be removed, only removing {1} out of {2} amount with reason {3}",
                    id, item.Amount, amount, reason);
                InventoryItems.Remove(id);
                return;
            }

            if (leftoverAmount == 0)
            {
                Logger.DebugInfo("Removed item {0} from inventory with reason {1}", id, reason);
                InventoryItems.Remove(id);
                return;
            }

            item.Amount -= amount;
            Logger.DebugInfo("Removed amount from item {0} with reason {1}, x{2} left", id, reason, amount);
            return;
        }

        Logger.DebugError("Could not find item data of item {0} to remove with reason {1}", itemDatabaseLocation,
            reason);
    }

    public int GetItemAmount(int id)
    {
        if (InventoryItems.TryGetValue(id, out InventoryItem item))
        {
            return item.Amount;
        }

        return 0;
    }

    public ItemData GetItemData(int id)
    {
        return ItemData[id];
    }
}