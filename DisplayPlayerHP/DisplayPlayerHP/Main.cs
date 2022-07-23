using BepInEx;
using HarmonyLib;

namespace DisplayPlayerHP
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "blumaye.displayplayerhp";
        public const string PluginName = "Display Player HP";
        public const string PluginVersion = "1.1.0";

        Harmony harmony = new Harmony(PluginGUID);

        public void Awake()
        {
            harmony?.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }
    }
}