using HarmonyLib;
using System.Collections.Generic;

namespace TradingSkill
{
    public static class TraderPatch
    {
        [HarmonyPatch(typeof(Trader), nameof(Trader.GetAvailableItems))]
        public static class Trader_GetAvailableItems_Prefix
        {
            public static bool Prefix(Trader __instance, ref List<Trader.TradeItem> __result)
            {
                __result = __instance.m_items;
                return false;
            }
        }
    }
}
