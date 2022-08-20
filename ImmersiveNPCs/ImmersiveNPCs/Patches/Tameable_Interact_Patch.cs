using HarmonyLib;
using UnityEngine;

namespace ImmersiveNPCs.Patches
{
    class Tameable_Interact_Patch
    {
		[HarmonyPatch(typeof(Tameable), nameof(Tameable.Interact))]
		public static class Patch
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

				if (!__instance.IsFriendly())
				{
					return true;
				}

				var friendly = __instance.gameObject.GetComponent<Friendly>();
				if (Input.GetKey(KeyCode.LeftShift))
				{
					if (friendly.isInUse)
					{
						Player.m_localPlayer.Message(MessageHud.MessageType.Center, "Npc is busy at the moment");
						return false;
					}

					// friendly.isInUse = true;

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

	}
}
