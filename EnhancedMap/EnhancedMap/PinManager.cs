using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Log = Jotunn.Logger;

namespace EnhancedMap
{
    class PinManager
    {
		/*
		public static Dictionary<string, Sprite> CreaturesList = new Dictionary<string, Sprite>()
		{
			{ "RRRN_Bandit_Leader_T1(Clone)", null },
			{ "RRRN_Bandit_Leader_T2(Clone)", null },
			{ "RRRN_Bandit_Leader_T3(Clone)", null },
			{ "RRRN_Bandit_Leader_T4(Clone)", null },
			{ "RRRN_Bandit_Leader_T5(Clone)", null },
			{ "RRRN_Bandit_Leader_T6(Clone)", null },
			{ "RRRM_SkeletonT3Captain(Clone)", Main.SkeletonSprite },
			{ "WendigoSwamp(Clone)", null },
			{ "Troll(Clone)", Main.TrollSprite },
			{ "RRR_TrollTosser(Clone)", null },
			{ "RRRM_GDElderBrute(Clone)", null },
			{ "RRRM_ObsidianGolem(Clone)", Main.GolemSprite },
			{ "StoneGolem(Clone)", Main.GolemSprite },
			{ "Serpent(Clone)", Main.SerpentSprite },
			{ "Skeleton(Clone)", Main.SkeletonSprite },
			{ "Wolf(Clone)", Main.WolfSprite },
			{ "Greydwarf(Clone)", Main.GreydwarfSprite },
			{ "Hatchling(Clone)", Main.HatchlingSprite },
			{ "Draugr(Clone)", Main.DraugrSprite },
			{ "Boar(Clone)", Main.BoarSprite },
			{ "Deer(Clone)", Main.DeerSprite },
			{ "Bear(Clone)", Main.BearSprite },
		};

		public static Dictionary<string, Sprite> BossList = new Dictionary<string, Sprite>()
		{
			{ "Eikthyr", Main.EikthyrSprite},
			{ "gd_king", Main.ElderSprite },
			{ "Bonemass", Main.BonemassSprite },
			{ "Dragon", Main.ModerSprite },
			{ "GoblinKing", Main.YagluthSprite }
		};
		*/

		public static Minimap.PinType FriendlyPinType = Minimap.PinType.Death;
		public static Minimap.PinType BossPinType = Minimap.PinType.Boss;
		public static Minimap.PinType EnemyPinType = Minimap.PinType.RandomEvent;
		public static Minimap.PinType NestPinType = Minimap.PinType.Icon4;

		public static Sprite FriendlyPinSprite = Minimap.instance.GetSprite(Minimap.PinType.Icon3);
		public static Sprite BossPinSprite = Minimap.instance.GetSprite(Minimap.PinType.Boss);
		public static Sprite EnemyPinSprite = Minimap.instance.GetSprite(Minimap.PinType.RandomEvent);
		public static Sprite NestPinSprite = Minimap.instance.GetSprite(Minimap.PinType.Icon4);

		public static Minimap.PinData AddCustomPin(Vector3 pos, Sprite sprite, float PinSize = 24f)
		{
			Minimap.PinData pinData = new Minimap.PinData();

			if (pinData.m_uiElement != null)
			{
				Object.Destroy(pinData.m_uiElement.gameObject);
			}

			if (sprite != null)
			{
				GameObject gameObject = Object.Instantiate(Minimap.instance.m_pinPrefab);
				pinData.m_icon = sprite;
				pinData.m_iconElement = gameObject.GetComponent<Image>();
				pinData.m_iconElement.sprite = pinData.m_icon;
			}
			else
			{
				pinData.m_icon = Minimap.instance.GetSprite(Minimap.PinType.Boss);
			}

			pinData.m_type = Minimap.PinType.Boss;
			pinData.m_name = "";
			pinData.m_pos = pos;
			pinData.m_save = false;
			pinData.m_checked = false;
			pinData.m_ownerID = 0L;
			pinData.m_worldSize = PinSize;

			Minimap.instance.m_pins.Add(pinData);
			return pinData;
		}

		public static Minimap.PinData AddEnemyPin(Vector3 pos)
		{
			Minimap.PinData pinData = new Minimap.PinData()
			{
				m_type = EnemyPinType,
				m_icon = EnemyPinSprite,
				m_name = "",
				m_pos = pos,
				m_save = false,
				m_checked = false,
				m_ownerID = 0L
			};
			Minimap.instance.m_pins.Add(pinData);
			return pinData;
		}

		public static Minimap.PinData AddFriendlyPin(Vector3 pos)
		{
			GameObject gameObject = Object.Instantiate(Minimap.instance.m_pinPrefab);
			Minimap.PinData pinData = new Minimap.PinData()
			{
				m_iconElement = gameObject.GetComponent<Image>(),
				m_type = FriendlyPinType,
				m_icon = FriendlyPinSprite,
				m_name = "",
				m_pos = pos,
				m_save = false,
				m_checked = false,
				m_ownerID = 0L
			};

			Minimap.instance.m_pins.Add(pinData);
			return pinData;
		}

		public static Minimap.PinData AddBossPin(Vector3 pos)
		{
			Minimap.PinData pinData = new Minimap.PinData()
			{
				m_type = BossPinType,
				m_icon = BossPinSprite,
				m_name = "",
				m_pos = pos,
				m_save = false,
				m_checked = false,
				m_ownerID = 0L
			};
			Minimap.instance.m_pins.Add(pinData);
			return pinData;
		}

		public static Minimap.PinData AddNestPin(Vector3 pos)
		{
			Minimap.PinData pinData = new Minimap.PinData()
			{
				m_type = NestPinType,
				m_icon = NestPinSprite,
				m_name = "",
				m_pos = pos,
				m_save = false,
				m_checked = false,
				m_ownerID = 0L
			};
			Minimap.instance.m_pins.Add(pinData);
			return pinData;
		}

		public static IEnumerable<Minimap.PinData> GetFriendlyPins()
        {
			return Minimap.instance.m_pins.Where(x => x.m_type == FriendlyPinType && x.m_icon == FriendlyPinSprite);
		}

		public static void CheckEventPins()
		{
			foreach (Minimap.PinData pinData in Main.mobs)
			{
				Minimap.instance.RemovePin(pinData);
			}
			Main.mobs.Clear();

			if (RandEventSystem.instance.GetCurrentRandomEvent() != null || RandEventSystem.instance.m_activeEvent != null)
			{
				foreach (MonsterAI monsterAI in Helpers.GetEnemies(20f))
				{
					if (monsterAI.HuntPlayer() && !monsterAI.m_character.IsBoss())
					{
						Minimap.PinData pin = AddEnemyPin(monsterAI.transform.position);
						Main.mobs.Add(pin);
					}
				}
			}
		}

		public static void CheckFriendlyPins()
        {
			for (int i = Main.friendlies.Count - 1; i >= 0; i--)
			{
				Friendly friendly = Main.friendlies[i];
				if (friendly.character != null)
				{
					if (friendly.IsPositionChanges())
					{
						friendly.Move();
					}
				}
				else
				{
					friendly.OnDeath();
					Main.friendlies.RemoveAt(i);
				}
			}
		}

		public static void CheckBossPins()
        {
			for (int i = Main.bosses.Count - 1; i >= 0; i--)
			{
				Boss boss = Main.bosses[i];
				if (boss.character != null)
				{
					if (boss.IsPositionChanges())
					{
						boss.Move();
					}
				}
				else
				{
					boss.OnDeath();
					Main.bosses.RemoveAt(i);
				}
			}
		}

		public static void CheckNestPins()
        {
			for (int i = Main.nests.Count - 1; i >= 0; i--)
			{
				Nest nest = Main.nests[i];
				if (nest.destructible == null)
				{
					nest.Destroy();
					Main.nests.RemoveAt(i);
				}
			}
		}

		public static void DestroyPins()
		{
			Main.friendlies.Clear();

			var pins = Minimap.instance.m_pins.Where(x => x.m_type == FriendlyPinType).ToArray();
			for (int i = pins.Count() - 1; i >= 0; i--)
			{
				Minimap.instance.RemovePin(pins[i]);
			}
		}
	}
}
