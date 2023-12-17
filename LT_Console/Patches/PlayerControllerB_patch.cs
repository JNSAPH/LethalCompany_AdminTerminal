using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;

namespace LT_Console.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerB_patch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        static void Postfix_Update(PlayerControllerB __instance)
        {
            if (__instance != null && Terminal_patch.sprintUnlim)
            {
                __instance.sprintMeter = 1.0f;
            }
        }
    }
}
