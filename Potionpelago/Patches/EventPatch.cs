using HarmonyLib;
using Potionpelago.CopyPastas;

namespace Potionpelago.Patches;

[PatchAll]
public static class EventPatch
{
    public static EventTypes CurrentEvent;

    [HarmonyPatch(typeof(EventManager), "ActivateEvent"), HarmonyPrefix]
    public static void Events(EventTypes x)
    {
        CurrentEvent = x;
        // Core.Log.Msg($"Event: [{CurrentEvent}]");
    }
}