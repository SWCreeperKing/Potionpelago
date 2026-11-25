using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Potionpelago.CopyPastas;
using Potionpelago.Patches;
using UnityEngine;
using static Potionpelago.Archipelago.ItemHandler;
using static Potionpelago.Patches.PotionPatch;

namespace Potionpelago.Archipelago;

public static class PotionClient
{
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
            // foreach (var pd in Potions.Values)
            foreach (var pc in SceneSingleton<PotionsManager>.Instance.potionControllersList)
            {
                pc.gameObject.SetActive(
                    ItemsUnlocked.Contains($"Unlock {PotionTypeToName[pc.potionCategoryType]} Potions")
                    && ItemsUnlocked.Contains($"Unlock {PuzzleTypeToName[pc.sortingPuzzleType]} Puzzles")
                );
                // Core.Log.Msg($"[{PotionTypeToName[pd.Color]}] - [{PuzzleTypeToName[pd.Puzzle]}] : [{pd.Controller.gameObject.activeSelf}]");
            }
            // Core.Log.Msg($"[{Potions.Count}] ({Potions.Values.Count(pd => pd.Controller.gameObject.activeSelf)})");
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