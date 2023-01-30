using System.Text;
using UnityEngine;

namespace ImmersiveNPCs
{
	public class Friendly : MonoBehaviour
    {
		public Character character;
		public Humanoid humanoid;
		public MonsterAI monsterAI;
		public ZNetView zNetView;

		public Vector3 position;
		public Minimap.PinData pin;

		public bool isInUse = false;

		public void Awake()
		{
			character = gameObject.GetComponent<Character>();
			humanoid = gameObject.GetComponent<Humanoid>(); 
			monsterAI = gameObject.GetComponent<MonsterAI>();
			zNetView = humanoid.GetComponent<ZNetView>();
			position = character.transform.position;
			pin = PinManager.AddFriendlyPin(position);
			Main.friendlies.Add(this);
		}

		public void OnDeath()
		{
			if (pin != null)
			{
				Minimap.instance.RemovePin(pin);
			}
		}

		public void UpdatePinPosition()
		{
			if (pin != null)
			{
				pin.m_pos = character.transform.position;
			}
		}

		/*
		public void Update()
        {
			float dt = Time.fixedDeltaTime;
			LayerMask m_autoPickupMask = LayerMask.GetMask("item");
			float m_autoPickupRange = 5f;

			if (character.IsTeleporting())
			{
				return;
			}

			Vector3 vector = character.transform.position + Vector3.up;
			Collider[] array = Physics.OverlapSphere(vector, m_autoPickupRange, m_autoPickupMask);
			foreach (Collider collider in array)
			{
				if (!collider.attachedRigidbody)
				{
					continue;
				}

				ItemDrop component = collider.attachedRigidbody.GetComponent<ItemDrop>();
				if (component == null || !component.m_autoPickup || !component.GetComponent<ZNetView>().IsValid())
				{
					continue;
				}

				if (!component.CanPickup())
				{
					component.RequestOwn();
				}
				else
				{
					if (component.InTar())
					{
						continue;
					}

					component.Load();

					if (!humanoid.m_inventory.CanAddItem(component.m_itemData) || component.m_itemData.GetWeight() + humanoid.m_inventory.GetTotalWeight() > Helpers.GetMaxWeight(humanoid))
					{
						continue;
					}

					float num = Vector3.Distance(component.transform.position, vector);
					if (num > Vector3.Distance(component.transform.position, Player.m_localPlayer.transform.position))
					{
						continue;
					}

					if (num <= m_autoPickupRange)
					{
						if (num < 0.3f)
						{
							Pickup(component.gameObject);
							return;
						}

						Vector3 vector2 = Vector3.Normalize(vector - component.transform.position);
						float num2 = 15f;
						component.transform.position = component.transform.position + vector2 * num2 * dt;
					}
				}
			}
		}
		*/

		public void Pickup(GameObject go)
		{
			ItemDrop component = go.GetComponent<ItemDrop>();

			if (component == null)
			{
				return;
			}

			component.Load();

			if (!component.CanPickup())
			{
				return;
			}

			/*
			if (humanoid.m_inventory.ContainsItem(component.m_itemData))
			{
				return;
			}
			*/

			bool flag = humanoid.m_inventory.AddItem(component.m_itemData);

			if (humanoid.m_nview.GetZDO() == null)
			{
				UnityEngine.Object.Destroy(go);
				return;
			}

			if (!flag)
			{
				return;
			}

			ZNetScene.instance.Destroy(go);

			// humanoid.m_pickupEffects.Create(humanoid.transform.position, Quaternion.identity);
			// SaveInventory(humanoid);
			// humanoid.m_inventory.UpdateTotalWeight();
		}

		public void SaveInventory()
		{
			ZPackage pkg = new ZPackage();
			humanoid.m_inventory.Save(pkg);
			string backpackBase64 = pkg.GetBase64();
			zNetView.GetZDO().Set("backpack", backpackBase64);
		}

		public void LoadInventory()
		{
			string backpackBase64 = zNetView.GetZDO().GetString("backpack");
			if (!string.IsNullOrEmpty(backpackBase64))
			{
				ZPackage pkg = new ZPackage(backpackBase64);
				humanoid.m_inventory.Load(pkg);
			}
		}

		public void UpdateInventory(Inventory backpackInventory = null, Container backpackContainer = null)
		{
			var inventory = humanoid.m_inventory.m_inventory;

			foreach (ItemDrop.ItemData item in inventory)
			{
				if (Helpers.isBow(item))
				{
					item.m_shared.m_aiAttackRange = 10f;
				}
			}

			if (backpackInventory != null)
			{
				var backpack = backpackInventory.m_inventory;

				foreach (ItemDrop.ItemData item in backpack)
				{
					if (Helpers.isBow(item))
					{
						item.m_shared.m_aiAttackRange = 10f;
					}
				}

				inventory.Clear();
				inventory.AddRange(backpack);

				foreach (ItemDrop.ItemData item in inventory)
				{
					if (Helpers.isBow(item))
					{
						item.m_shared.m_aiAttackRange = 10f;
					}
				}

				backpackContainer.m_inventory.UpdateTotalWeight();
			}

			humanoid.m_inventory.UpdateTotalWeight();
			SaveInventory();
		}

		public void UpdateEquipment()
		{
			var inventory = humanoid.m_inventory.m_inventory;
			var rightItem = humanoid.m_rightItem;
			var leftItem = humanoid.m_leftItem;
			var helmetItem = humanoid.m_helmetItem;
			var chestItem = humanoid.m_chestItem;
			var legItem = humanoid.m_legItem;
			var shoulderItem = humanoid.m_shoulderItem;

			if (!inventory.Contains(leftItem))
			{
				humanoid.UnequipItem(leftItem);
			}

			if (!inventory.Contains(rightItem))
			{
				humanoid.UnequipItem(rightItem);
			}

			if (!inventory.Contains(helmetItem))
			{
				humanoid.UnequipItem(helmetItem);
			}

			if (!inventory.Contains(chestItem))
			{
				humanoid.UnequipItem(chestItem);
			}

			if (!inventory.Contains(legItem))
			{
				humanoid.UnequipItem(legItem);
			}

			if (!inventory.Contains(shoulderItem))
			{
				humanoid.UnequipItem(shoulderItem);
			}

			if (chestItem == null)
			{
				var chest = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Chest);
				if (chest != null)
				{
					humanoid.EquipItem(chest);
				}
			}

			if (helmetItem == null)
			{
				var helmet = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Helmet);
				if (helmet != null)
				{
					humanoid.EquipItem(helmet);
				}
			}

			if (legItem == null)
			{
				var leg = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Legs);
				if (leg != null)
				{
					humanoid.EquipItem(leg);
				}
			}

			if (shoulderItem == null)
			{
				var shoulder = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Shoulder);
				if (shoulder != null)
				{
					humanoid.EquipItem(shoulder);
				}
			}

			var bow = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Bow);
			var shield = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.Shield);
			var weapon2H = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.TwoHandedWeapon);
			var weapon1H = Helpers.FindItem(inventory, ItemDrop.ItemData.ItemType.OneHandedWeapon);

			if (bow != null)
			{
				humanoid.EquipItem(bow);
				return;
			}

			if (weapon2H != null)
			{
				humanoid.EquipItem(weapon2H);
				return;
			}

			if (shield != null)
			{
				humanoid.EquipItem(shield);
			}

			if (weapon1H != null)
			{
				humanoid.EquipItem(weapon1H); 
				return;
			}
		}

		public void CreateTombstone()
        {
			if (humanoid.m_inventory.NrOfItems() != 0)
			{
				humanoid.UnequipAllItems();

				GameObject obj = Object.Instantiate(Player.m_localPlayer.m_tombstone, humanoid.GetCenterPoint(), humanoid.transform.rotation);

				Container container = obj.GetComponent<Container>();
				Inventory inventory = container.GetInventory();
				inventory.MoveInventoryToGrave(humanoid.m_inventory);
				inventory.m_name = humanoid.m_name;

				TombStone component = obj.GetComponent<TombStone>();
				component.Setup(humanoid.m_name, humanoid.GetInstanceID());
			}
		}

		public void HitNearTree()
        {
			var rightItem = humanoid.m_rightItem;

			if (rightItem != null)
			{
				if (rightItem.m_dropPrefab.name.ToLower().Contains("axe") && rightItem.m_shared.m_skillType == Skills.SkillType.Axes)
				{
					var colliders = Physics.OverlapSphere(humanoid.transform.position, rightItem.m_shared.m_aiAttackMaxAngle);

					foreach (var collider in colliders)
					{
						var tree = collider?.gameObject?.GetComponentInParent<TreeBase>();
						var log = collider?.gameObject?.GetComponentInParent<TreeLog>();
						var destructible = collider?.gameObject?.GetComponentInParent<Destructible>();
						if (tree != null)
						{
							monsterAI.LookAt(tree.transform.position);
							monsterAI.DoAttack(null, false);
						}
						else if (log != null)
						{
							monsterAI.LookAt(log.transform.position);
							monsterAI.DoAttack(null, false);
						}
						else if (destructible != null)
						{
							if (destructible.name.ToLower().Contains("stub"))
							{
								monsterAI.LookAt(destructible.transform.position);
								monsterAI.DoAttack(null, false);
							}
						}
					}
				}
			}
		}

		public void HitNearRock()
		{
			var rightItem = humanoid.m_rightItem;

			if (rightItem != null)
			{
				if (rightItem.m_dropPrefab.name.ToLower().Contains("pickaxe"))
				{
					var colliders = Physics.OverlapSphere(humanoid.transform.position, rightItem.m_shared.m_aiAttackMaxAngle);

					foreach (var collider in colliders)
					{
						var rock = collider?.gameObject?.GetComponentInParent<MineRock5>();
						if (rock != null)
						{
							monsterAI.LookAt(rock.transform.position);
							monsterAI.DoAttack(null, false);
						}
					}
				}
			}
		}
		
		public string GetHoverText()
		{
			string useKey = "[<color=yellow><b>$KEY_Use</b></color>]";
			string useKeyAll = "[<color=yellow><b>$KEY_AltPlace + $KEY_Use</b></color>]";

			var text = new StringBuilder();
			text.AppendLine(Localization.instance.Localize(character.m_name));

			var followTarget = monsterAI.GetFollowTarget();
			if (followTarget != null)
			{
				var name = followTarget.GetComponent<Player>().GetPlayerName();
				text.AppendLine(Localization.instance.Localize($"Follow by: <color=orange>{name}</color>"));
			}

			if (isInUse)
			{
				text.AppendLine($"<color=red>Busy</color>");
			}
			else
			{
				text.AppendLine($"<color=green>Free</color>");
			}

			text.AppendLine(Localization.instance.Localize($"{useKey} Take Along"));
			text.AppendLine(Localization.instance.Localize($"{useKeyAll} Open Inventory"));
			text.AppendLine(Localization.instance.Localize($"<color=yellow>Armor ({Helpers.GetArmor(humanoid)})</color>"));
			text.AppendLine(Localization.instance.Localize($"<color=yellow>Equipment</color> {Helpers.GetTotalWeight(humanoid)}"));
			text.AppendLine(Helpers.GetItems(humanoid));

			return text.ToString();
		}
	}
}
