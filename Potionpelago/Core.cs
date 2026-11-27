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

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);
        PotionClient.Init();

        Log.Msg("Initialized.");
    }

    public override void OnGUI()
    {
        if (EventPatch.CurrentEvent is not EventTypes.MainMenuLoaded) return;
        if (PotionClient.QuitToMainMenu)
        {
            PotionClient.QuitToMainMenu = false;
            PotionClient.Client.TryDisconnect();
            OverrideSaveFile();
        }
        APGui.Render();
    }

    public override void OnUpdate()
    {
        if (EventPatch.CurrentEvent is not (EventTypes.GameStart or EventTypes.GameResume or EventTypes.GamePause))
            return;
        PotionClient.Update();
    }

    public static void MainMenuStart()
    {
        try
        {
            MainMenuUI ??= Object.FindAnyObjectByType<MainMenuUI>();
            MainMenuUI.CallPrivateMethod("Start");
        }
        catch { }
    }

    public static void OverrideSaveFile(string saveDir = "")
    {
        SaveDirPatch.SaveDir = saveDir;
        try { Singleton<SaveSystem>.Instance.CallPrivateMethod("OnEnable"); }
        catch { }
        MainMenuStart();
        UIBackKeyManager.ClearQueue();
        PlayerPrefs.SetInt("OpenMainMenu", 1);
        Singleton<ScenesManager>.Instance.GoToGameScene();
    }
}