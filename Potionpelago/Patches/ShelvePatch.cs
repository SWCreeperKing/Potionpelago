using HarmonyLib;
using Potionpelago.CopyPastas;
using static Potionpelago.Archipelago.PotionClient;

namespace Potionpelago.Patches;

[PatchAll]
public class ShelvePatch
{
    [HarmonyPatch(typeof(ShelfController), "IsShelfCompletedRight"), HarmonyPostfix]
    public static void FinishPuzzle(ShelfController __instance, ref bool __result)
    {
        if (!__result) return;
        SendLocation(
            $"Complete {PotionTypeToName[__instance.cabinetController.potionCategoryType]} Potions: {PuzzleTypeToName[__instance.shelfSortingPuzzleType]} Puzzle"
        );
    }
}