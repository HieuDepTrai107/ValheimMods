using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class JotunnModStub : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.jotunnmodstub";
        public const string PluginName = "JotunnModStub";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<float> JumpForceConfig;

        Harmony harmony = new Harmony(PluginGUID);

        public void Awake()
        {
            ConfigDeploy();
            harmony?.PatchAll();
        }

        public void OnDestroy()
        {
            harmony?.UnpatchSelf();
        }

        [HarmonyPatch(typeof(Character), nameof(Character.Jump))]
        class Jump_Patch
        {
            static void Prefix(ref float ___m_jumpForce)
            {
                ___m_jumpForce = JumpForceConfig.Value;
                print($"Jump force: {___m_jumpForce}");
            }
        }

        public void ConfigDeploy()
        {
            Config.SaveOnConfigSet = true;
            JumpForceConfig = Config.Bind("Jump Force", "Jump Force", 10f, "Jump Force");
        }

    }
}