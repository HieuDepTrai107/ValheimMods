using HarmonyLib;

namespace EnhancedBosses
{
    class EnemyHud_TestShow
    {
        [HarmonyPatch(typeof(EnemyHud), nameof(EnemyHud.TestShow))]
        public static class EnemyHud_TestShow_Postfix
        {
            public static void Postfix(ref Character c, ref bool __result)
            {
                if (c.name.Contains("RRRM_EikthyrClone"))
                {
                    __result = false;
                }
            }
        }
    }
}
