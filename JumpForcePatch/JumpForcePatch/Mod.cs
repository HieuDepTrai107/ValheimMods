using BepInEx;
using HarmonyLib;
using BepInEx.Configuration;

namespace JumpForcePatch
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class JumpForcePatch : BaseUnityPlugin
    {
        public const string PluginGUID = "yarikmix.jumpforcepatch";
        public const string PluginName = "JumpForcePatch";
        public const string PluginVersion = "1.0.0";

        public static ConfigEntry<int> JumpForceConfig;

        Harmony harmony = new Harmony(PluginGUID);

        void Awake()
        {
            ConfigDeploy();
            harmony.PatchAll();
        }

        private void OnDestroy()
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
            JumpForceConfig = Config.Bind("Jump Force", "Jump Force", 10, new ConfigDescription("Jump Force", new AcceptableValueRange<int>(0, 100)));
        }

    }
}