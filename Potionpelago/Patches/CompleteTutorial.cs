using HarmonyLib;
using Potionpelago.CopyPastas;

namespace Potionpelago.Patches;

[PatchAll]
public static class CompleteTutorial
{
    [HarmonyPatch(typeof(TutorialManager), "StartTutorial"), HarmonyPostfix]
    public static void FinishTutorial(TutorialManager __instance)
    {
        if (!__instance.isTutorialCompleted) __instance.CompleteTutorial(false);
        SceneSingleton<TutorialManagerUI>.Instance.HidePanel();
    }
}