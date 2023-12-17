using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LT_Console.Patches;
using System;

namespace LT_Console
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class LT_ConsoleModBase : BaseUnityPlugin
    {
        public const string modGUID = "com.jnsaph.LT_Console";
        private const string modName = "LT_Console";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static LT_ConsoleModBase Instance { get; private set; }
        internal ManualLogSource ManualLogSource { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this; // Set the instance to the current object.
            }
            else
            {
                DestroyImmediate(this); // Destroy any additional instances that may be created.
                return;
            }

            ManualLogSource = Logger;
            ManualLogSource.LogInfo("LT_ConsoleModBase started.");

            harmony.PatchAll(typeof(LT_ConsoleModBase));
            harmony.PatchAll(typeof(PlayerControllerB_patch));
            harmony.PatchAll(typeof(Terminal_patch));
        }
    }
}
