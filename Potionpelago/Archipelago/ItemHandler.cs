using Archipelago.MultiClient.Net.Models;

namespace Potionpelago.Archipelago;

public static class ItemHandler
{
    public static HashSet<string> ItemsUnlocked = [];
    public static bool UpdatePotions;

    public static void HandleItem(ItemInfo item)
    {
        if (item.ItemName is "Cat Food") return;
        ItemsUnlocked.Add(item.ItemName);
        UpdatePotions = true;
    }
}