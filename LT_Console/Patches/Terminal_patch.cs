using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System.Collections;
using System.Text;
using TMPro;
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

        static void add_to_chat(string chatMessage)
        {
            HUDManager.Instance.lastChatMessage = chatMessage;
            HUDManager.Instance.PingHUDElement(HUDManager.Instance.Chat, 4f);
            if (HUDManager.Instance.ChatMessageHistory.Count >= 4)
            {
                HUDManager.Instance.chatText.text.Remove(0, HUDManager.Instance.ChatMessageHistory[0].Length);
                HUDManager.Instance.ChatMessageHistory.Remove(HUDManager.Instance.ChatMessageHistory[0]);
            }

            StringBuilder stringBuilder = new StringBuilder(chatMessage);
            stringBuilder.Replace("[playerNum0]", StartOfRound.Instance.allPlayerScripts[0].playerUsername);
            stringBuilder.Replace("[playerNum1]", StartOfRound.Instance.allPlayerScripts[1].playerUsername);
            stringBuilder.Replace("[playerNum2]", StartOfRound.Instance.allPlayerScripts[2].playerUsername);
            stringBuilder.Replace("[playerNum3]", StartOfRound.Instance.allPlayerScripts[3].playerUsername);
            chatMessage = stringBuilder.ToString();
            string item = ((!string.IsNullOrEmpty(HUDManager.Instance.chatText.text)) ? "\n" : "") + chatMessage;
            HUDManager.Instance.ChatMessageHistory.Add(item);
            HUDManager.Instance.chatText.text += "";
            for (int i = 0; i < HUDManager.Instance.ChatMessageHistory.Count; i++)
            {
                TextMeshProUGUI textMeshProUGUI = HUDManager.Instance.chatText;
                textMeshProUGUI.text = textMeshProUGUI.text + "\n" + HUDManager.Instance.ChatMessageHistory[i];
            }
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

                        add_to_chat("Unlimited Sprint has been set to " + sprintUnlim + "!");
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo(input_command);
                        break;

                    case "set_Money":
                        if (command_parts.Length > 1)
                        {
                            int money = int.Parse(command_parts[1]);
                            __instance.groupCredits = money;
                            __instance.SyncGroupCreditsClientRpc(money, __instance.numberOfItemsInDropship);

                            add_to_chat("Money has been set to " + money + "!");
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

                            add_to_chat("Jump has been set to " + jump + "!");
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

                            add_to_chat("Speed has been set to " + speed + "!");
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Speed: " + speed);
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("for Player: " + playerController.playerUsername);
                        }
                        break;
                    case "reset_QoutaTo":
                        if (command_parts.Length > 1)
                        {
                            int quota = int.Parse(command_parts[1]);
                            TimeOfDay.Instance.SyncNewProfitQuotaClientRpc(quota, 0, 0);

                            add_to_chat("Quota has been reset to " + quota + "!");
                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Quota: " + quota);
                        }
                        break;
                    case "chat_test":
                        if (command_parts.Length > 1)
                        {
                            string message = command_parts[1];

                            add_to_chat(message);

                            LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Message: " + message);
                        }
                        break;

                    default:
                        // Log all custom Commands
                        LT_ConsoleModBase.Instance.ManualLogSource.LogInfo("Unknown Command, try:" +
                            "\n > set_UnlimSprint true/false" +
                            "\n > set_Money <amount>" +
                            "\n > set_Jump <amount>" +
                            "\n > set_Speed <amount>" +
                            "\n > reset_QoutaTo <quota>" +
                            "\n > chat_test <msg>");
                        break;
                }
            }
        }
    }
}
