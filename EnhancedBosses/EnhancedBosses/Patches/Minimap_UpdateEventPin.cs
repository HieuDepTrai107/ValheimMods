using HarmonyLib;

namespace EnhancedBosses
{
    class Minimap_UpdateEventPin
    {
        [HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdateEventPin))]
        public static class UpdateEventMobMinimapPinsPatch
        {
            public static void Postfix()
            {
                PinManager.CheckBossPins();
            }
        }
    }
}
