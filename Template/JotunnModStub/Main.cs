using BepInEx;
using HarmonyLib;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.jotunnmodstub";
        public const string PluginName = "JotunnModStub";
        public const string PluginVersion = "1.0.0";

        public static Harmony harmony = new Harmony(PluginGUID);

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