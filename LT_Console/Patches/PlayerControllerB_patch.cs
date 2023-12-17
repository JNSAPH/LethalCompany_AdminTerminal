using GameNetcodeStuff;
using HarmonyLib;
using JetBrains.Annotations;
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
            // Check if the instance is already set and is the same
            if (__instance != null && !Terminal_patch.IsPlayerControllerSet(__instance))
            {
                Terminal_patch.SetPlayerController(__instance);
            }

            if (Terminal_patch.sprintUnlim)
            {
                __instance.sprintMeter = 1f;
            }

            if (Terminal_patch.playerIsUnkillable)
            {
                __instance.health = 100;
            }
        }
    }
}
