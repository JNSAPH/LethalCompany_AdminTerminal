using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;

namespace LT_Console.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class Terminal_patch
    {
        static public bool sprintUnlim = false;

        [HarmonyPatch("ParsePlayerSentence")]
        [HarmonyPostfix]
        static void Postfix_ParsePlayerSentence(Terminal __instance)
        {
            if (__instance != null && __instance.screenText != null)
            {
                string s = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);

                LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Command: " + s);

                switch (s)
                {
                    case "unlimSprintOn":
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("unlimSprintOn");
                        sprintUnlim = true;
                        break;
                    case "unlimSprintOff":
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("unlimSprintOff");
                        sprintUnlim = false;
                        break;

                    default:
                        // Handle the default case if no recognized command is found
                        break;
                }
            }
        }
    }
}
