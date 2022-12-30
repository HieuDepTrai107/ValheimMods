using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Log = Jotunn.Logger;

namespace TradingSkill
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "blumaye.tradingskill";
        public const string PluginName = "TradingSkill";
        public const string PluginVersion = "1.0.0";

        public static Harmony harmony = new Harmony(PluginGUID);

        public static ConfigEntry<float> maxDiscount;

        public static ConfigEntry<float> sellExpMultiplier;

        public static ConfigEntry<float> buyExpMultiplier;

        public static ConfigEntry<float> a;
        public static ConfigEntry<float> b;
        public static ConfigEntry<float> c;
        public static ConfigEntry<float> d;
        public static ConfigEntry<float> e;
        public static ConfigEntry<float> f;


        public void Awake()
        {
            CreateConfigValues();

            TradingSkill.AddToSkills();

            harmony?.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        public void CreateConfigValues()
        {
            Config.SaveOnConfigSet = true;
            maxDiscount = Config.Bind("Скидка", "Максимальная скидка", 100f, "Максимальная скидка (в процентах)");
            sellExpMultiplier = Config.Bind("Скидка", "Множитель опыта при покупке", 1f);
            buyExpMultiplier = Config.Bind("Скидка", "Множитель опыта при продаже", 1f);
            a = Config.Bind("Test", "Test a", 0f);
            b = Config.Bind("Test", "Test b", 0f);
            c = Config.Bind("Test", "Test c", 0f);
            d = Config.Bind("Test", "Test d", 0f);
            e = Config.Bind("Test", "Test e", 0f);
            f = Config.Bind("Test", "Test f", 0f);
        }
    }
}