using HarmonyLib;

namespace EnhancedBosses
{
    class BaseAI_CanUseAttack
    {
        [HarmonyPatch(typeof(BaseAI), nameof(BaseAI.CanUseAttack))]
        public static class BaseAI_CanUseAttack_Postfix
        {
            public static void Postfix(BaseAI __instance, ItemDrop.ItemData item, ref bool __result)
            {
                if (Helpers.GetBoss(__instance) is Boss { } boss)
                {
                    if (__result)
                    {
                        __result = boss.CanUseAttack(item);
                    }
                }
            }
        }
    }
}
