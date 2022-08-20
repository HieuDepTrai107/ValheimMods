using HarmonyLib;

namespace ImmersiveNPCs.Patches
{
    class HoverText
    {       
		[HarmonyPatch(typeof(Tameable), nameof(Tameable.GetHoverText))]
		public static class Tameable_GetHoverText_Postfix
		{
			public static void Postfix(Tameable __instance, ref string __result)
			{
				if (__instance.IsFriendly())
				{
					Friendly friendly = __instance.gameObject.GetComponent<Friendly>();
					__result = friendly.GetHoverText();
				}
			}
		}
	}
}
