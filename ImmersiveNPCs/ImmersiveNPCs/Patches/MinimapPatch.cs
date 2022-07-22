using HarmonyLib;
using UnityEngine;

namespace ImmersiveNPCs.Patches
{
    class MinimapPatch
    {
		[HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdateEventPin))]
		public static class UpdateEventMobMinimapPinsPatch
		{
			public static void Postfix()
			{
				PinManager.CheckFriendlyPins();
			}
		}

		[HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdatePins))]
		public static class MinimapUpdatePins
		{
			public static void Postfix()
			{
				foreach (Minimap.PinData pinData in PinManager.GetFriendlyPins())
				{
					pinData.m_iconElement.color = Color.green;
				}
			}
		}
	}
}
