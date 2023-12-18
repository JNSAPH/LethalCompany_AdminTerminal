using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using Unity.Netcode;
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
            if (pc.playerUsername.Contains("Player"))
            {
                LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("PlayerController is not set, waiting for Player to join...");
                return;
            }
            playerController = pc;
            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("PlayerController set to User: " + pc.playerUsername);
        }
        public static bool IsPlayerControllerSet(PlayerControllerB controller)
        {
            return playerController != null;
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
                    case "set_UnlimSprint":
                        if (command_parts.Length > 1)
                        {
                            if (command_parts[1] == "true")
                                sprintUnlim = true;
                            else if (command_parts[1] == "false")
                                sprintUnlim = false;
                        }

                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo(input_command);
                        break;

                    case "set_Money":
                        if (command_parts.Length > 1)
                        {
                            int money = int.Parse(command_parts[1]);
                            __instance.groupCredits = money;
                            __instance.SyncGroupCreditsClientRpc(money, __instance.numberOfItemsInDropship);

                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Money: " + money);
                        }
                        break;

                    case "set_Jump":
                        if (command_parts.Length > 1)
                        {
                            float jump = float.Parse(command_parts[1]);
                            playerController.jumpForce = jump;

                            if (jump == 0f)
                            {
                                playerController.jumpForce = currentController.jumpForce;
                            }

                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Jump: " + jump);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("for Player: " + playerController.playerUsername);
                        }
                        break;

                    case "set_Speed":
                        if (command_parts.Length > 1)
                        {
                            float speed = float.Parse(command_parts[1]);
                            playerController.movementSpeed = speed;

                            if(speed == 0)
                            {
                                playerController.movementSpeed = currentController.movementSpeed;
                            }

                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Speed: " + speed);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("for Player: " + playerController.playerUsername);
                        }
                        break;
                    case "reset_QoutaTo":
                        if (command_parts.Length > 1)
                        {
                            int quota = int.Parse(command_parts[1]);
                            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(quota, 0, 0);

                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Quota: " + quota);
                        }
                        break;

                    default:
                        // Log all custom Commands
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Unknown Command, try:" +
                            "\n > set_UnlimSprint true/false" +
                            "\n > set_Money <amount>" +
                            "\n > set_Jump <amount>" +
                            "\n > set_Speed <amount>" +
                            "\n > reset_QoutaTo <quota>");
                        break;
                }
            }
        }
    }
}
