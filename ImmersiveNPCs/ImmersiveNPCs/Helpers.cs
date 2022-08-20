using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;
using System.Text.RegularExpressions;
using System.Linq;
using Random = UnityEngine.Random;

namespace ImmersiveNPCs
{
	public static class Helpers
    {
		public static bool IsFriendly(this Tameable tameable)
		{
			var name = tameable.name.ToLower();
			return Main.friendlyPrefixes.Value.Split(',').ToList().Any(e => name.Contains(e.Trim().ToLower()));
		}

		public static bool IsFriendly(this Humanoid humanoid)
		{
			string name = humanoid.name.ToLower();
			return Main.friendlyPrefixes.Value.Split(',').ToList().Any(e => name.Contains(e.Trim().ToLower()));
		}

		public static bool IsFriendly(this Character character)
		{
			string name = character.name.ToLower();
			return Main.friendlyPrefixes.Value.Split(',').ToList().Any(e => name.Contains(e.Trim().ToLower()));
		}

		public static bool IsFriendly(this BaseAI baseAI)
		{
			string name = baseAI.name.ToLower();
			return Main.friendlyPrefixes.Value.Split(',').ToList().Any(e => name.Contains(e.Trim().ToLower()));
		}

		public static bool IsNPC(this Character character)
		{
			return character != null ? character.name.Contains("RRRN") : false;
		}

		public static bool isBow(ItemDrop.ItemData item)
		{
			if (item == null) return false;
			return item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow;
		}

		public static bool isBow(GameObject prefab)
		{
			if (prefab == null) return false;
			return prefab.GetComponent<ItemDrop>().m_itemData.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Bow;
		}

		public static bool isShield(ItemDrop.ItemData item)
		{
			if (item == null) return false;
			return item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Shield;
		}

		public static bool isArrow(GameObject prefab)
		{
			return prefab.name.ToLower().Contains("arrow");
		}

		public static bool IsFood(ItemDrop.ItemData item)
		{
			return item.m_shared.m_itemType == ItemDrop.ItemData.ItemType.Consumable && (item.m_shared.m_food > 0f || item.m_shared.m_foodStamina > 0f);
		}

		public static bool IsArmor(ItemDrop.ItemData item)
		{
			var type = item.m_shared.m_itemType;
			return type == ItemDrop.ItemData.ItemType.Helmet || type == ItemDrop.ItemData.ItemType.Chest || type == ItemDrop.ItemData.ItemType.Legs || type == ItemDrop.ItemData.ItemType.Shoulder;
		}

		public static void GiveArrow(GameObject prefab, Humanoid humanoid)
		{
			var count = Random.Range(Main.minArrows.Value, Main.maxArrows.Value);
			var item = prefab.GetComponent<ItemDrop>().m_itemData;

			if (item.m_dropPrefab == null)
            {
				item.m_dropPrefab = prefab;
            }

			for (int i = 0; i < count; i++)
			{
				humanoid.m_inventory.m_inventory.Add(item);
			}
		}

		public static float GetMaxWeight(Humanoid humanoid)
		{
			float result;
			bool flag = Config.maxWeight.TryGetValue(humanoid.name, out result);
			if (flag)
				return result;
			else
				return 100f;
		}

		public static string GetTotalWeight(Humanoid humanoid)
		{
			float totalWeight = humanoid.m_inventory.GetTotalWeight();
			float maxWeight = GetMaxWeight(humanoid);
			if (totalWeight < maxWeight)
			{
				return $"<color=yellow>({totalWeight.ToString("0.0")} / {GetMaxWeight(humanoid)})</color>";
			}
			else
			{
				string color = Mathf.Sin(Time.time * 10f) > 0f ? "yellow" : "red";
				return $"<color={color}>({totalWeight.ToString("0.0")} / {GetMaxWeight(humanoid)})</color>";
			}
		}

		public static float GetArmor(this Character character)
		{
			var humanoid = character.GetComponent<Humanoid>();
			var equipedItems = humanoid.m_inventory.GetEquipedtems();
			float armor = 0f;

			foreach (var item in equipedItems)
			{
				if (IsArmor(item))
				{
					armor += item.GetArmor();
				}

				if (isShield(item))
				{
					armor += item.m_shared.m_blockPower / 5;
				}
			}

			return armor;
		}

		public static string GetItems(Humanoid humanoid)
		{
			var text = "";
			var items = new Dictionary<ItemDrop.ItemData, int>();

			foreach (var item in humanoid.m_inventory.m_inventory)
			{
				var originalItem = GetOriginalItem(item);
				if (items.ContainsKey(originalItem))
				{
					items[originalItem] += item.m_stack;
				}
				else
				{
					items.Add(originalItem, item.m_stack);
				}
			}

			foreach (var item in items.Keys)
			{
				string stack = items[item] > 1 ? $" x {items[item]}" : "";
				text += $"{Localization.instance.Localize(item.m_shared.m_name)} {stack}\n";
			}

			return text.ToString().TrimEnd(new char[] { '\n' });
		}


		public static ItemDrop.ItemData GetOriginalItem(ItemDrop.ItemData item)
		{
			if (item == null) return null;
			if (item.m_dropPrefab == null) return item;
			var rawPrefabName = item.m_dropPrefab.name;
			var prefabname = rawPrefabName.Contains("@") ? Regex.Match(rawPrefabName, @"\@(.+?)\@").Groups[1].ToString() : rawPrefabName;
			var originalItem = PrefabManager.Instance.GetPrefab(prefabname)?.GetComponent<ItemDrop>().m_itemData;
			return originalItem;
		}

		public static GameObject GetOriginalPrefab(GameObject prefab)
		{
			if (prefab == null) return null;
			var rawPrefabName = prefab.name;
			var prefabname = rawPrefabName.Contains("@") ? Regex.Match(rawPrefabName, @"\@(.+?)\@").Groups[1].ToString() : rawPrefabName;
			var originalPrefab = PrefabManager.Instance.GetPrefab(prefabname);
			var itemData = originalPrefab.GetComponent<ItemDrop>().m_itemData;
			itemData.m_durability = Random.Range(0.25f, 1f) * itemData.GetMaxDurability();
			return originalPrefab;
		}

		public static ItemDrop.ItemData FindWeapon(List<ItemDrop.ItemData> inventory, ItemDrop.ItemData.ItemType itemType = ItemDrop.ItemData.ItemType.OneHandedWeapon)
		{
			foreach (ItemDrop.ItemData item in inventory)
			{
				if (item.IsWeapon() && item.m_shared.m_itemType == itemType) return item;
			}
			return null;
		}

		public static ItemDrop.ItemData FindItem(List<ItemDrop.ItemData> inventory, params ItemDrop.ItemData.ItemType[] types)
		{
			foreach (var type in types)
			{
				foreach (var item in inventory)
				{
					if (item.m_shared.m_itemType == type) return item;
				}
			}

			return null;
		}

		public static List<ItemDrop.ItemData> FindItems(List<ItemDrop.ItemData> inventory, Skills.SkillType skillType, int max = 2)
		{
			var items = new List<ItemDrop.ItemData>();

			foreach (var item in inventory)
			{
				if (item.m_shared.m_skillType == skillType)
				{
					items.Add(item);
					if (items.Count == max)
					{
						return items;
					}
				}
			}

			return items;
		}

		public static ItemDrop.ItemData FindFood(List<ItemDrop.ItemData> inventory)
		{
			foreach (ItemDrop.ItemData item in inventory)
			{
				if (IsFood(item)) return item;
			}

			return null;
		}

		public static List<Friendly> GetFriendlies(float radius = 10f, bool checkFollow = true)
		{
			var player = Player.m_localPlayer;
			var friendlies = new List<Friendly>();
			List<Character> list = new List<Character>();
			Character.GetCharactersInRange(player.transform.position, radius, list);
			foreach (Character character in list)
			{
				var friendly = character.gameObject.GetComponent<Friendly>();
				if (friendly != null)
				{
					if (checkFollow)
                    {
						var monsterAI = character.GetComponent<MonsterAI>();
						if (monsterAI && monsterAI.GetFollowTarget() && monsterAI.GetFollowTarget().Equals(player.gameObject))
						{
							friendlies.Add(friendly);
						}
                    }
					else
                    {
						friendlies.Add(friendly);
					}
				}
			}
			return friendlies;
		}
	}
}
