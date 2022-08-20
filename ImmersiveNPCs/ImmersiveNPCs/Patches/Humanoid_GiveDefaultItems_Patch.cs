using HarmonyLib;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ImmersiveNPCs.Patches
{
    class Humanoid_GiveDefaultItems_Patch
    {

		[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GiveDefaultItems))]
		public static class Patch
		{
			public static bool Prefix(Humanoid __instance)
			{
				if (__instance.IsFriendly())
				{
					var friendly = __instance.gameObject.GetComponent<Friendly>();
					friendly.LoadInventory();

					if (__instance.m_inventory.m_inventory.Count == 0)
					{
						GameObject[] defaultItems = __instance.m_defaultItems;
						Random.State state = Random.state;
						Random.InitState(__instance.m_nview.GetZDO().m_uid.GetHashCode());

						foreach (GameObject rawPrefab in defaultItems)
						{
							var prefab = Helpers.GetOriginalPrefab(rawPrefab);

							if (Helpers.isArrow(prefab))
							{
								Helpers.GiveArrow(prefab, __instance);
							}
							else
							{
								if (Helpers.isBow(prefab))
								{
									var item = prefab.GetComponent<ItemDrop>().m_itemData;
									item.m_shared.m_aiAttackRange = 10;
								}
								__instance.GiveDefaultItem(prefab);
							}
						}
						if (__instance.m_randomWeapon.Length + __instance.m_randomArmor.Length + __instance.m_randomShield.Length + __instance.m_randomArmor.Length + __instance.m_randomSets.Length == 0)
						{
							return false;
						}

						if (__instance.m_randomShield.Length != 0)
						{
							GameObject rawPrefab = __instance.m_randomShield[Random.Range(0, __instance.m_randomShield.Length)];
							var prefab = Helpers.GetOriginalPrefab(rawPrefab);
							if (prefab != null)
							{
								__instance.GiveDefaultItem(prefab);
							}
						}

						if (__instance.m_randomWeapon.Length != 0)
						{
							GameObject rawPrefab = __instance.m_randomWeapon[Random.Range(0, __instance.m_randomWeapon.Length)];
							if (rawPrefab != null)
							{
								var prefab = Helpers.GetOriginalPrefab(rawPrefab);
								if (prefab != null)
								{
									if (Helpers.isBow(prefab))
									{
										var item = prefab.GetComponent<ItemDrop>().m_itemData;
										item.m_shared.m_aiAttackRange = 10;
									}
									__instance.GiveDefaultItem(prefab);
								}
							}

						}

						if (__instance.m_randomArmor.Length != 0)
						{
							GameObject rawPrefab = __instance.m_randomArmor[Random.Range(0, __instance.m_randomArmor.Length)];
							var prefab = Helpers.GetOriginalPrefab(rawPrefab);
							if (prefab != null)
							{
								__instance.GiveDefaultItem(prefab);
							}
						}

						if (__instance.m_randomSets.Length != 0)
						{
							var setItems = __instance.m_randomSets[Random.Range(0, __instance.m_randomSets.Length)].m_items;
							foreach (GameObject rawPrefab in setItems)
							{
								var prefab = Helpers.GetOriginalPrefab(rawPrefab);
								__instance.GiveDefaultItem(prefab);
							}
						}

						Random.state = state;

						friendly.SaveInventory();
					}

					return false;
				}

				return true;
			}
		}

	}
}
