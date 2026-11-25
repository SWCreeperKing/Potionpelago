using HarmonyLib;
using Potionpelago.Archipelago;
using Potionpelago.CopyPastas;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Potionpelago.Patches;

[PatchAll]
public static class PotionPatch
{
    public static Dictionary<PotionCategoryType, string> PotionTypeToName = new()
    {
        [PotionCategoryType.Healing] = "Red", [PotionCategoryType.Stamina] = "Lime", [PotionCategoryType.Mana] = "Blue",
        [PotionCategoryType.Poison] = "Purple", [PotionCategoryType.Fire] = "Orange", [PotionCategoryType.Ice] = "Cyan",
        [PotionCategoryType.Lightning] = "Yellow", [PotionCategoryType.Earth] = "Brown",
        [PotionCategoryType.Holy] = "Beige", [PotionCategoryType.Charm] = "Pink", [PotionCategoryType.Stealth] = "Gray",
        [PotionCategoryType.Necromancy] = "Black", [PotionCategoryType.Nature] = "Green",
    };

    public static Dictionary<string, PotionCategoryType> NameToPotionType = PotionTypeToName.ToDictionary(
        kv => kv.Value, kv => kv.Key
    );

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

    public static Dictionary<string, SortingPuzzleType> NameToPuzzleType = PuzzleTypeToName.ToDictionary(
        kv => kv.Value, kv => kv.Key
    );

    // public static Dictionary<int, PotionData> Potions = [];

    // [HarmonyPatch(typeof(PotionController), "ShelfPotion"), HarmonyPrefix]
    // public static void Place(PotionController __instance, ShelfController shelfController, int currentOrderOnShelf)
    // {
    //     Core.Log.Msg($"[{__instance.potionCategoryType}] - [{__instance.sortingPuzzleType}]");
    // }

    // [HarmonyPatch(typeof(PotionController), "LoadItem"), HarmonyPostfix]
    // public static void Disable(PotionController __instance, ItemSaveData itemSaveData)
    // {
    //     var data = new PotionData(__instance);
    //     Potions[data.GetHashCode()] = data;
    // }

    // [HarmonyPatch(typeof(GameManager), "ContinueGame"), HarmonyPostfix]
    // public static void GrabInfo()
    // {
    //     Potions.Clear();
    //     if (!PotionClient.Client.IsConnected) return;
    //     foreach (var pc in Object.FindObjectsByType<PotionController>(FindObjectsSortMode.InstanceID))
    //     {
    //         if (!SceneSingleton<PotionsManager>.Instance.potionControllersList.Contains(pc))
    //         {
    //             Object.DestroyImmediate(pc.gameObject);
    //             continue;
    //         }
    //         
    //         if (!pc.gameObject.activeSelf) continue;
    //         // pc.gameObject.SetActive(false);
    //         var data = new PotionData(pc);
    //         var hash = data.GetHashCode();
    //         Potions.TryAdd(hash, data);
    //     }
    //     ItemHandler.UpdatePotions = true;
    // }
}

//
// public class PotionUi : GuiBuilder
// {
//     public ScrollWindow Window;
//     public Dictionary<PotionCategoryType, bool> Toggles = [];
//
//     protected override void InitGUI()
//     {
//         Window = new ScrollWindow(50, 50, 200, 400,"Potion Toggles");
//         foreach (var type in Enum.GetValues(typeof(PotionCategoryType)).Cast<PotionCategoryType>())
//         {
//             Toggles[type] = true;
//             Window.AddChild(
//                 new Toggle(
//                     new SetterGetter<bool>(
//                         () => Toggles[type], b =>
//                         {
//                             if (Toggles[type] != b) TogglePotionType(type);
//                         }
//                     ), $"Toggle [{type}]", 30
//                 )
//             );
//         }
//     }
//
//     protected override void OnGUI() => Window.Render();
//
//     public void TogglePotionType(PotionCategoryType type)
//     {
//         Toggles[type] = !Toggles[type];
//         foreach (var potion in PotionPatch.Potions.Values.Where(p => p.Controller.potionCategoryType == type))
//             potion.Controller.gameObject.SetActive(Toggles[type]);
//     }
// }

public readonly struct PotionData(PotionController pc) : IEquatable<PotionData>
{
    public readonly string Name = pc.gameObject.name;
    public readonly int Order = pc.orderInSet;
    public readonly PotionCategoryType Color = pc.potionCategoryType;
    public readonly SortingPuzzleType Puzzle = pc.sortingPuzzleType;
    public readonly PotionController Controller = pc;

    public override int GetHashCode() => HashCode.Combine(Order, Color, Puzzle, Name);

    public bool Equals(PotionData other) => Order == other.Order && Color == other.Color && Puzzle == other.Puzzle
                                            && Equals(Controller, other.Controller);

    public override bool Equals(object obj) => obj is PotionData other && Equals(other);
}