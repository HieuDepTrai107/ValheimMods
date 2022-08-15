using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedMap
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "blumaye.enhancedmap";
        public const string PluginName = "EnhancedMap";
        public const string PluginVersion = "1.0.0";

        public static Harmony harmony = new Harmony(PluginGUID);

        public static ConfigEntry<bool> FriendlyDetection;
        public static ConfigEntry<bool> BossDetection;
        public static bool NestDetetction = false;

        public static List<Minimap.PinData> mobs = new();
        public static List<Friendly> friendlies = new();
        public static List<Boss> bosses = new();
        public static List<Nest> nests = new();

        public static Sprite TinOreSprite;
        public static Sprite CopperOreSprite;
        public static Sprite ObsidianOreSprite;
        public static Sprite SilverOreSprite;
        public static Sprite FlametalOreSprite;
        public static Sprite FineTreeSprite;

        public void Awake()
        {
            CreateConfigValues();
            LoadAssets();
            PrefabManager.OnVanillaPrefabsAvailable += SetupCharacters;
            PrefabManager.OnVanillaPrefabsAvailable += SetupTreeBase;
            PrefabManager.OnVanillaPrefabsAvailable += SetupMineRock;
            PrefabManager.OnVanillaPrefabsAvailable += SetupDestructible;
            harmony?.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        public void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;
            FriendlyDetection = Config.Bind("Карта", "Отображать союзников", true);
            BossDetection = Config.Bind("Карта", "Отображать боссов", true);
        }

        public static void LoadAssets()
        {
            TinOreSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/TinOre.png");
            CopperOreSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/CopperOre.png");
            ObsidianOreSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/ObsidianOre.png");
            SilverOreSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/SilverOre.png");
            FlametalOreSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/FlametalOre.png");
            FineTreeSprite = AssetUtils.LoadSpriteFromFile("EnhancedMap/Assets/FineWoodSprite.png");
        }

        public static void SetupCharacters()
        {
            foreach (var character in Resources.FindObjectsOfTypeAll<Character>())
            {
                if (character.IsBoss())
                {
                    character.gameObject.AddComponent<Boss>();
                }
            }

            PrefabManager.OnVanillaPrefabsAvailable -= SetupCharacters;
        }

        public static void SetupTreeBase()
        {
            foreach (var treeBase in Resources.FindObjectsOfTypeAll<TreeBase>())
            {
                if (treeBase.name.Contains("Birch"))
                {
                    treeBase.gameObject.AddComponent<Tree>();
                }
            }

            PrefabManager.OnVanillaPrefabsAvailable -= SetupTreeBase;
        }

        public static void SetupMineRock()
        {
            foreach (var mineRock in Resources.FindObjectsOfTypeAll<MineRock>())
            {
                if (mineRock.name.Contains("MineRock_Meteorite"))
                {
                    mineRock.gameObject.AddComponent<Ore>();
                }
            }

            foreach (var mineRock5 in Resources.FindObjectsOfTypeAll<MineRock5>())
            {
                if (mineRock5.name.Contains("rock4_copper_frac") || mineRock5.name.Contains("rock3_silver_frac"))
                {
                    mineRock5.gameObject.AddComponent<Ore>();
                }
            }

            PrefabManager.OnVanillaPrefabsAvailable -= SetupMineRock;
        }

        public static void SetupDestructible()
        { 
            List<string> objs = new() { "MineRock_Tin", "rock4_copper", "MineRock_Obsidian", "rock3_silver", "silvervein" };
               
            foreach (var destructible in Resources.FindObjectsOfTypeAll<Destructible>())
            {
                if (destructible.name.Contains("Spawner_GreydwarfNest"))
                {
                    // destructible.gameObject.AddComponent<Nest>();
                }

                if (objs.Any(e => destructible.name.Contains(e)))
                {
                    destructible.gameObject.AddComponent<Ore>();
                }
            }

            PrefabManager.OnVanillaPrefabsAvailable -= SetupDestructible;
        }
    }
}