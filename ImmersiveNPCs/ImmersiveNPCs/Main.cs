using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;
using System.Collections.Generic;

namespace ImmersiveNPCs
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "blumaye.immersivenpcs";
        public const string PluginName = "ImmersiveNPCs";
        public const string PluginVersion = "1.0.0";

        public static Harmony harmony = new Harmony(PluginGUID);

        public static ConfigEntry<bool> CreateTombstones;
        public static ConfigEntry<int> minArrows;
        public static ConfigEntry<int> maxArrows;
        public static ConfigEntry<int> maxWeightT1;
        public static ConfigEntry<int> maxWeightT2;
        public static ConfigEntry<int> maxWeightT3;
        public static ConfigEntry<int> maxWeightT4;
        public static ConfigEntry<int> maxWeightT5;
        public static ConfigEntry<int> maxWeightT6;
        public static ConfigEntry<string> friendlyPrefixes;

        public static List<Friendly> friendlies = new();


        public void Awake()
        {
            CreateConfigValues();
            harmony?.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        public void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;
            CreateTombstones = Config.Bind("NPC", "Create tombstone", true, "Create tombstone after death");
            minArrows = Config.Bind("NPC", "Min amount of arrows", 10);
            maxArrows = Config.Bind("NPC", "Max amount of arrows", 30);
            maxWeightT1 = Config.Bind("NPC", "Max weight Т1", 25);
            maxWeightT2 = Config.Bind("NPC", "Max weight Т2", 50);
            maxWeightT3 = Config.Bind("NPC", "Max weight Т3", 100);
            maxWeightT4 = Config.Bind("NPC", "Max weight Т4", 200);
            maxWeightT5 = Config.Bind("NPC", "Max weight Т5", 300);
            maxWeightT6 = Config.Bind("NPC", "Max weight Т6", 500);
            friendlyPrefixes = Config.Bind("NPC", "Prefixes", "Friendly, friendly");
        }
    }
}