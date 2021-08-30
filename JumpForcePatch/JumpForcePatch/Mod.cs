using BepInEx;
using HarmonyLib;

namespace JotunnModStub
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid)]
    public class JotunnModStub : BaseUnityPlugin
    {
        public const string PluginGUID = "com.jotunn.jotunnmodstub";
        public const string PluginName = "JotunnModStub";
        public const string PluginVersion = "1.0.0";

        Harmony harmony = new Harmony(PluginGUID);

        private void Awake()
        {
            harmony?.PatchAll();
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
                ___m_jumpForce = 50f;
                print($"Jump force: {___m_jumpForce}");
            }
        }

    }
}