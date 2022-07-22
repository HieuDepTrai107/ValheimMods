using HarmonyLib;
using UnityEngine;

namespace ImmersiveNPCs.Patches
{
	public class Patches
	{
		[HarmonyPatch(typeof(Character), nameof(Character.Awake))]
		public static class Character_Awake_Postfix
		{
			public static void Postfix(Character __instance)
			{
				if (Helpers.IsFriendly(__instance))
				{
					__instance.gameObject.AddComponent<Friendly>();
				}
			}
		}

		[HarmonyPatch(typeof(Tameable), nameof(Tameable.SetName))]
		public static class Tameable_SetName_Patch
		{
			public static bool Prefix(Tameable __instance)
			{
				return !Helpers.IsFriendly(__instance);
			}
		}

		[HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
		public static class Tameable_Interact_Patch
		{
			public static string backpackName = "NPC's inventory";

			public static bool Prefix(Tameable __instance, Humanoid user, bool hold, ref bool __result)
			{
				__result = false;

				if (!__instance.m_nview.IsValid())
				{
					return false;
				}

				if (hold)
				{
					return false;
				}

				var friendly = __instance.gameObject.GetComponent<Friendly>();
				if (friendly != null && Input.GetKey(KeyCode.LeftShift))
				{
					if (friendly.isInUse)
					{
						Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Npc is busy at the moment");
						return false;
					}

					friendly.isInUse = true;

					Humanoid humanoid = __instance.GetComponent<Humanoid>();
					Inventory friendlyInventory = humanoid.m_inventory;

					friendly.LoadInventory();

					Inventory backpackInventory = new Inventory(backpackName, null, 5, 4);
					Container backpackContainer = Player.m_localPlayer.gameObject.GetComponent<Container>();

					backpackInventory.m_onChanged += () => friendly.UpdateInventory(backpackInventory, backpackContainer);
					backpackInventory.m_inventory.AddRange(friendlyInventory.m_inventory);

					if (backpackContainer == null) backpackContainer = Player.m_localPlayer.gameObject.AddComponent<Container>();
					backpackContainer.m_name = backpackName;
					AccessTools.FieldRefAccess<Container, Inventory>(backpackContainer, "m_inventory") = backpackInventory;
					InventoryGui.instance.Show(backpackContainer);

					backpackContainer.m_inventory.UpdateTotalWeight();
					friendlyInventory.UpdateTotalWeight();

					return false;
				}

				return true;
			}
		}

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

		[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.GiveDefaultItems))]
		public static class Humanoid_GiveDefaultItems_Patch
		{
			public static bool Prefix(Humanoid __instance)
			{
				var friendly = __instance.gameObject.GetComponent<Friendly>();

				if (friendly != null)
				{
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

		[HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UpdateEquipment))]
		public static class Humanoid_UpdateEquipment_Patch
		{
			public static void Postfix(Humanoid __instance)
			{
				var friendly = __instance.gameObject.GetComponent<Friendly>();
				if (friendly != null)
				{
					friendly.UpdateEquipment();
				}
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.IsEncumbered))]
		public static class Character_IsEncumbered_Patch
		{
			public static void Postfix(Character __instance, ref bool __result)
			{
				var friendly = __instance.gameObject.GetComponent<Friendly>();
				if (friendly != null)
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

		[HarmonyPatch(typeof(Character), nameof(Character.RPC_Damage))]
		public static class Character_RPC_Damage_Patch
		{
			public static bool Prefix(Character __instance, HitData hit)
			{
				if (!Helpers.IsNPC(__instance)) return true;

				if (__instance.IsDebugFlying() || !__instance.m_nview.IsOwner() || __instance.GetHealth() <= 0f || __instance.IsDead() || __instance.IsTeleporting() || __instance.InCutscene() || (hit.m_dodgeable && __instance.IsDodgeInvincible()))
				{
					return false;
				}
				Character attacker = hit.GetAttacker();
				if ((hit.HaveAttacker() && attacker == null) || (__instance.IsPlayer() && !__instance.IsPVPEnabled() && attacker != null && attacker.IsPlayer()))
				{
					return false;
				}
				if (attacker != null && !attacker.IsPlayer())
				{
					float difficultyDamageScalePlayer = Game.instance.GetDifficultyDamageScalePlayer(__instance.transform.position);
					hit.ApplyModifier(difficultyDamageScalePlayer);
				}
				__instance.m_seman.OnDamaged(hit, attacker);
				if (__instance.m_baseAI != null && !__instance.m_baseAI.IsAlerted() && hit.m_backstabBonus > 1f && Time.time - __instance.m_backstabTime > 300f)
				{
					__instance.m_backstabTime = Time.time;
					hit.ApplyModifier(hit.m_backstabBonus);
					__instance.m_backstabHitEffects.Create(hit.m_point, Quaternion.identity, __instance.transform);
				}
				if (__instance.IsStaggering() && !__instance.IsPlayer())
				{
					hit.ApplyModifier(2f);
					__instance.m_critHitEffects.Create(hit.m_point, Quaternion.identity, __instance.transform);
				}
				if (hit.m_blockable && __instance.IsBlocking())
				{
					__instance.BlockAttack(hit, attacker);
				}
				__instance.ApplyPushback(hit);
				if (!string.IsNullOrEmpty(hit.m_statusEffect))
				{
					StatusEffect statusEffect = __instance.m_seman.GetStatusEffect(hit.m_statusEffect);
					if (statusEffect == null)
					{
						statusEffect = __instance.m_seman.AddStatusEffect(hit.m_statusEffect);
					}
					else
					{
						statusEffect.ResetTime();
					}
					if (statusEffect != null && attacker != null)
					{
						statusEffect.SetAttacker(attacker);
					}
				}
				HitData.DamageModifiers damageModifiers = __instance.GetDamageModifiers();
				hit.ApplyResistance(damageModifiers, out var significantModifier);

				float bodyArmor = Helpers.GetArmor(__instance);
				hit.ApplyArmor(bodyArmor);
				__instance.DamageArmorDurability(hit);

				float poison = hit.m_damage.m_poison;
				float fire = hit.m_damage.m_fire;
				float spirit = hit.m_damage.m_spirit;
				hit.m_damage.m_poison = 0f;
				hit.m_damage.m_fire = 0f;
				hit.m_damage.m_spirit = 0f;
				__instance.ApplyDamage(hit, showDamageText: true, triggerEffects: true, significantModifier);
				__instance.AddFireDamage(fire);
				__instance.AddSpiritDamage(spirit);
				__instance.AddPoisonDamage(poison);
				__instance.AddFrostDamage(hit.m_damage.m_frost);
				__instance.AddLightningDamage(hit.m_damage.m_lightning);

				return false;
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
		public static class Character_OnDeath_Patch
		{
			public static void Postfix(Character __instance)
			{
				if (!Main.CreateTombstones.Value) return;

				var friendly = __instance.gameObject.GetComponent<Friendly>();
				if (friendly == null) return;
				friendly.CreateTombstone();
			}
		}
	}
}
