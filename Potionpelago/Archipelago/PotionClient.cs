using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Potionpelago.CopyPastas;
using UnityEngine;
using static Potionpelago.Archipelago.ItemHandler;
using Object = UnityEngine.Object;

namespace Potionpelago.Archipelago;

public static class PotionClient
{
    public static Dictionary<PotionCategoryType, string> PotionTypeToName = new()
    {
        [PotionCategoryType.Healing] = "Red", [PotionCategoryType.Stamina] = "Lime", [PotionCategoryType.Mana] = "Blue",
        [PotionCategoryType.Poison] = "Purple", [PotionCategoryType.Fire] = "Orange", [PotionCategoryType.Ice] = "Cyan",
        [PotionCategoryType.Lightning] = "Yellow", [PotionCategoryType.Earth] = "Brown",
        [PotionCategoryType.Holy] = "Beige", [PotionCategoryType.Charm] = "Pink", [PotionCategoryType.Stealth] = "Gray",
        [PotionCategoryType.Necromancy] = "Black", [PotionCategoryType.Nature] = "Green",
    };

    public static Dictionary<SortingPuzzleType, string> PuzzleTypeToName = new()
    {
        [SortingPuzzleType.PotionSize_5] = "Size", [SortingPuzzleType.PotionLiquidSize_5] = "Liquid",
        [SortingPuzzleType.PotionCorkSize_5] = "Cork", [SortingPuzzleType.BasicNumbers_5] = "Number",
        [SortingPuzzleType.Animals_5] = "Animal", [SortingPuzzleType.CrowInteraction_5] = "Crow",
        [SortingPuzzleType.Spices_5] = "Spice", [SortingPuzzleType.Flowers_8] = "Flower",
        [SortingPuzzleType.ItemsInPotion_8] = "Content", [SortingPuzzleType.RomanNumerals_8] = "Numeral",
        [SortingPuzzleType.MoonsShapes_8] = "Moon", [SortingPuzzleType.Compass_8] = "Compass",
        [SortingPuzzleType.Clock_8] = "Clock", [SortingPuzzleType.GeometricShapes_8] = "Shape",
        [SortingPuzzleType.Chess_8] = "Chess", [SortingPuzzleType.MusicNotes_8] = "Notes",
        [SortingPuzzleType.LightInteraction_8] = "Candle", [SortingPuzzleType.MusicInteraction_8] = "Music",
        [SortingPuzzleType.WeightInteraction_8] = "Weight", [SortingPuzzleType.ChemicalGrades_8] = "Chemical",
        [SortingPuzzleType.PotionParticles_8] = "Crystal", [SortingPuzzleType.IngredientPrice_8] = "Ingredient",
        [SortingPuzzleType.Cards_8] = "Card", [SortingPuzzleType.Weather_8] = "Weather",
        [SortingPuzzleType.Elements_8] = "Element", [SortingPuzzleType.GibberishLanguage_8] = "Greek",
        [SortingPuzzleType.Rainbow_8] = "Color", [SortingPuzzleType.HeroAdventure_8] = "Adventure",
    };

    public static bool QuitToMainMenu = false;

    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static string GameUUID = "";

    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () => Core.Log.Error("Lost Connection to Ap");

        Client.OnConnectionEvent += _ =>
        {
            try
            {
                ItemsUnlocked.Clear();
                GameUUID = (string)Client.SlotData["uuid"];
                Core.OverrideSaveFile($"{Directory.GetCurrentDirectory()}/Archipelago/{GameUUID}");
                CheckGoal();
            }
            catch (Exception e) { Core.Log.Error(e); }
        };

        Client.ItemHandlerInitialized += handler => { handler.OnItemReceived += (item, _) => HandleItem(item); };

        Client.OnConnectionErrorReceived += (e, _) => Core.Log.Error(e);
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "Shelve the Potions", ItemsHandlingFlags.AllItems);
    }

    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));

    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;
            Client.UpdateItemHandler();

            if (!UpdatePotions) return;

            foreach (var pc in SceneSingleton<PotionsManager>.Instance.potionControllersList)
            {
                var go = pc.gameObject;
                var active =
                    ItemsUnlocked.Contains($"Unlock {PotionTypeToName[pc.potionCategoryType]} Potions")
                    && ItemsUnlocked.Contains($"Unlock {PuzzleTypeToName[pc.sortingPuzzleType]} Puzzles");

                go.GetComponent<MeshRenderer>().enabled = active;
                go.GetComponent<PotionController>().enabled = active;
                go.GetComponent<CapsuleCollider>().enabled = active;
                foreach (var child in go.GetChildren()) child.SetActive(active);
            }
            Core.Log.Msg($"Size: [{SceneSingleton<PotionsManager>.Instance.potionControllersList.Count}]");
            UpdatePotions = false;
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    public static void SendLocation(string loc)
    {
        if (Client is null || !Client.IsConnected) return;
        if (!Client.IsMissingLocation(loc))
        {
            CheckGoal();
            return;
        }
        Client.SendLocation(loc);
        CheckGoal();
    }

    public static void CheckGoal()
    {
        if (Client is null) return;
        if (!Client.IsConnected) return;
        if (Client.MissingLocations.Any()) return;
        Client.TryGoal();
    }
}