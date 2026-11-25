using HarmonyLib;
using Potionpelago.CopyPastas;
using Steamworks;
using UnityEngine;

namespace Potionpelago.Patches;

[PatchAll]
public static class SaveDirPatch
{
    public static string SaveDir = "";

    [HarmonyPatch(typeof(SaveSystem), "GetSaveDir"), HarmonyPrefix]
    public static bool GetSaveDir(ref string __result, string folderName = "")
    {
        var rawSaveDir = SaveDir is "" ? Application.persistentDataPath : SaveDir;

        try
        {
            var path = rawSaveDir;
            if (SteamManager.Initialized) path = $"{path}/{SteamUser.GetSteamID().ToString()}";
            if (folderName != "") path = $"{path}/{folderName}";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            __result = path;
        }
        catch { __result = rawSaveDir; }
        return false;
    }
}