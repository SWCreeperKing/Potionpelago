using MelonLoader;
using Potionpelago.Archipelago;
using Potionpelago.CopyPastas;
using Potionpelago.Patches;
using UnityEngine;
using Object = UnityEngine.Object;

[assembly: MelonInfo(typeof(Potionpelago.Core), "Potionpelago", "0.1.0", "SW_CreeperKing", null)]
[assembly: MelonGame("Knight Owl Games", "Shelve the Potions!")]

namespace Potionpelago;

public class Core : MelonMod
{
    public static MelonLogger.Instance Log;
    public static APGui APGui = new();
    public static MainMenuUI MainMenuUI;

    // CharacterInventory

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);

        // KeyRegistry.TryAddKey(
        //     KeyCode.P, Key.P, () =>
        //     {
        //         var data = PotionPatch.Potions.Values.ToArray();
        //         Dictionary<PotionCategoryType, HashSet<SortingPuzzleType>> dict = [];
        //
        //         foreach (var potion in data)
        //         {
        //             if (!dict.TryGetValue(potion.Color, out var list)) dict[potion.Color] = list = [];
        //             list.Add(potion.Puzzle);
        //         }
        //
        //         File.WriteAllLines(
        //             "Puzzles.txt",
        //             dict.SelectMany(kv => kv.Value.Select(p
        //                     => $"{PotionPatch.PotionTypeToName[kv.Key]}|{PotionPatch.PuzzleTypeToName[p]}"
        //                 )
        //             )
        //         );
        //     }
        // );

        PotionClient.Init();

        Log.Msg("Initialized.");
    }

    public override void OnGUI()
    {
        if (EventPatch.CurrentEvent is EventTypes.MainMenuLoaded) APGui.Render();
    }

    public override void OnUpdate()
    {
        KeyRegistry.Update();
        if (EventPatch.CurrentEvent is EventTypes.GameStart or EventTypes.GameResume or EventTypes.GamePause)
            PotionClient.Update();
    }

    public static void MainMenuStart()
    {
        try
        {
            MainMenuUI ??= Object.FindAnyObjectByType<MainMenuUI>();
            MainMenuUI.CallPrivateMethod("Start");
        } catch {}
    }

    public static void OverrideSaveFile(string saveDir = "")
    {
        SaveDirPatch.SaveDir = saveDir;
        try { Singleton<SaveSystem>.Instance.CallPrivateMethod("OnEnable"); }
        catch {}
        MainMenuStart();
        UIBackKeyManager.ClearQueue();
        PlayerPrefs.SetInt("OpenMainMenu", 1);
        Singleton<ScenesManager>.Instance.GoToGameScene();
    }
}