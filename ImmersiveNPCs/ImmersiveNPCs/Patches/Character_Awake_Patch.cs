using HarmonyLib;

namespace ImmersiveNPCs
{
    class Character_Awake_Patch
	{
		[HarmonyPatch(typeof(Character), nameof(Character.Awake))]
		public static class Patch
		{
			public static void Postfix(Character __instance)
			{
				if (__instance.IsFriendly())
                {
					__instance.gameObject.AddComponent<Friendly>();
				}
			}
		}
	}
}
