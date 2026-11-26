using HarmonyLib;
using Potionpelago.Archipelago;
using Potionpelago.CopyPastas;

namespace Potionpelago.Patches;

[PatchAll]
public static class EventPatch
{
    public static EventTypes CurrentEvent;

    [HarmonyPatch(typeof(EventManager), "ActivateEvent"), HarmonyPrefix]
    public static void Events(EventTypes x) => CurrentEvent = x;

    [HarmonyPatch(typeof(PauseMenuUI), "GoToMainMenu"), HarmonyPostfix]
    public static void Clear() => PotionClient.QuitToMainMenu = true;
}