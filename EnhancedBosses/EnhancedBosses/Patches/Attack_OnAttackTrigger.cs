using EnhancedBosses.Bosses;
using HarmonyLib;

namespace EnhancedBosses
{
    class Attack_OnAttackTrigger
    {
        [HarmonyPatch(typeof(Attack), nameof(Attack.OnAttackTrigger))]
        public static class Attack_OnAttackTrigger_Prefix
        {
            public static bool Prefix(Attack __instance)
            {
                var character = __instance.m_character as Character;

                if (Helpers.GetBoss(character) is Boss { } boss)
                {
                    return boss.Process_Attack(__instance);
                }

                return true;
            }
        }

    }
}
