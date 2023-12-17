using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace LT_Console.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    internal class Terminal_patch
    {
        static public bool sprintUnlim = false;
        static private PlayerControllerB playerController;
        private static PlayerControllerB currentController = null;
        static public bool playerIsUnkillable = false;

        static public void SetPlayerController(PlayerControllerB pc)
        {
            playerController = pc;
        }
        public static bool IsPlayerControllerSet(PlayerControllerB controller)
        {
            return currentController == controller;
        }


        [HarmonyPatch("ParsePlayerSentence")]
        [HarmonyPostfix]
        static void Postfix_ParsePlayerSentence(Terminal __instance)
        {
            if (__instance != null && __instance.screenText != null)
            {
                string input_command = __instance.screenText.text.Substring(__instance.screenText.text.Length - __instance.textAdded);
                string[] command_parts = input_command.Split(' ');

                LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Command: " + input_command);
                   


                switch (command_parts[0])
                {
                    case "set_Sprint":
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo(input_command);
                        if (command_parts.Length > 1)
                        {
                            if (command_parts[1] == "true")
                            {
                                sprintUnlim = true;
                            }
                            else if (command_parts[1] == "false")
                            {

                                sprintUnlim = false;
                            }
                        }
                        break;
                    case "set_Money":
                        if (command_parts.Length > 1)
                        {
                            int money = int.Parse(command_parts[1]);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Money: " + money);
                            __instance.groupCredits = money;
                            __instance.SyncGroupCreditsClientRpc(money, __instance.numberOfItemsInDropship);
                        }
                        break;
                    case "set_Jump":
                        if (command_parts.Length > 1)
                        {
                            float jump = float.Parse(command_parts[1]);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Jump: " + jump);
                            playerController.jumpForce = jump;
                        }
                        break;
                    case "set_Speed":
                        if (command_parts.Length > 1)
                        {
                            float speed = float.Parse(command_parts[1]);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Speed: " + speed);
                            playerController.movementSpeed = speed;

                            if(speed == 0)
                            {
                                playerController.movementSpeed = currentController.movementSpeed;
                            }
                        }
                        break;
                    case "set_QuotaGoal":
                        if (command_parts.Length > 1)
                        {
                            int quota = int.Parse(command_parts[1]);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Quota: " + quota);
                            HUDManager.Instance.DisplayDaysLeft(quota);
                            TimeOfDay.Instance.quotaFulfilled = quota;
                            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(quota, 0, 0);
                        }
                        break;
                    default:
                        // Handle the default case if no recognized command is found
                        break;
                }
            }
        }
    }
}
