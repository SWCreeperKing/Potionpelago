using HarmonyLib;
using Potionpelago.Archipelago;
using Potionpelago.CopyPastas;

namespace Potionpelago.Patches;

[PatchAll]
public class ShelvePatch
{
    [HarmonyPatch(typeof(ShelfController), "IsShelfCompletedRight"), HarmonyPostfix]
    public static void FinishPuzzle(ShelfController __instance, ref bool __result)
    {
        if (!__result) return;
        PotionClient.SendLocation(
            $"Complete {PotionPatch.PotionTypeToName[__instance.cabinetController.potionCategoryType]} Potions: {PotionPatch.PuzzleTypeToName[__instance.shelfSortingPuzzleType]} Puzzle"
        );
    }
}