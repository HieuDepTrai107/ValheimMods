using HarmonyLib;

namespace ImmersiveNPCs.Patches
{
	public class Patches
	{
		[HarmonyPatch(typeof(Tameable), nameof(Tameable.SetName))]
		public static class Tameable_SetName_Patch
		{
			public static bool Prefix(Tameable __instance)
			{
				return !Helpers.IsFriendly(__instance);
			}
		}

		/*
		[HarmonyPatch(typeof(InventoryGui), nameof(InventoryGui.Hide))]
		public static class InventoryGui_CloseContainer_Patch
		{
			public static void Prefix(InventoryGui __instance)
			{
				if ((bool)__instance.m_currentContainer)
				{
					if (__instance.m_currentContainer.m_name == "NPC's inventory")
					{
						foreach (var friendly in Helpers.GetFriendlies(5f, false))
						{
							if (friendly.isInUse)
							{
								friendly.isInUse = false;
								return;
							}
						}
					}
				}
			}
		}
		*/

		[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UpdateEquipment))]
		public static class Humanoid_UpdateEquipment_Patch
		{
			public static void Postfix(Humanoid __instance)
			{
				if (__instance.IsFriendly())
				{
					Friendly friendly = __instance.gameObject.GetComponent<Friendly>();
					friendly.UpdateEquipment();
				}
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.IsEncumbered))]
		public static class Character_IsEncumbered_Patch
		{
			public static void Postfix(Character __instance, ref bool __result)
			{
				if (__instance.IsFriendly())
                {
					var humanoid = __instance.GetComponent<Humanoid>();
					var weight = humanoid.m_inventory.GetTotalWeight();
					var maxWeight = Helpers.GetMaxWeight(humanoid);

					if (weight > maxWeight)
					{
						__result = true;
					}
                }
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.ApplyDamage))]
		public static class Character_ApplyDamage_Prefix
		{
			public static void Prefix(Character __instance, HitData hit)
			{
				if (__instance.IsNPC())
                {
					float bodyArmor = __instance.GetArmor();
					hit.ApplyArmor(bodyArmor);
				}
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
		public static class Character_OnDeath_Patch
		{
			public static void Postfix(Character __instance)
			{
				if (Main.CreateTombstones.Value)
				{
					if (__instance.IsFriendly())
					{
						var friendly = __instance.gameObject.GetComponent<Friendly>();
						friendly.CreateTombstone();
					}
				}
			}
		}
	}
}
