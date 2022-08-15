using System.Collections.Generic;
using Terraheim.ArmorEffects;
using UnityEngine;

namespace EnhancedMap
{
	public class Ore : MonoBehaviour
	{
		public Minimap.PinData pin;
		public Sprite sprite;

		public Dictionary<string, Sprite> sprites = new()
		{
			{ "MineRock_Tin(Clone)", Main.TinOreSprite },
			{ "rock4_copper(Clone)", Main.CopperOreSprite },
			{ "rock4_copper_frac(Clone)", Main.CopperOreSprite },
			{ "MineRock_Obsidian(Clone)", Main.ObsidianOreSprite },
			{ "silvervein(Clone)", Main.SilverOreSprite },
			{ "rock3_silver(Clone)", Main.SilverOreSprite },
			{ "rock3_silver_frac(Clone)", Main.SilverOreSprite },
			{ "MineRock_Meteorite(Clone)", Main.FlametalOreSprite },
		};

		public void Awake()
		{
			sprite = sprites[gameObject.name];
		}

		public void OnDestroy()
		{
			if (pin != null && Minimap.instance != null)
			{
				Minimap.instance.RemovePin(pin);
			}
		}

		public void Update()
        {
			if (Player.m_localPlayer != null)
			{
				if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Mining Bonus"))
				{
					SE_MiningBonus statusEffect = Player.m_localPlayer.GetSEMan().GetStatusEffect("Mining Bonus") as SE_MiningBonus;

					if (pin == null)
					{
						if (Vector3.Distance(Player.m_localPlayer.transform.position, gameObject.transform.position) <= statusEffect.GetExploreRadius())
						{
							pin = PinManager.AddCustomPin(gameObject.transform.position, sprite);
                        }
					}
					else
					{
						if (Vector3.Distance(Player.m_localPlayer.transform.position, gameObject.transform.position) > statusEffect.GetExploreRadius())
						{
							Minimap.instance.RemovePin(pin);
						}
					}
                }
				else
				{
					if (pin != null)
					{
						Minimap.instance.RemovePin(pin);
					}
				}
            }
        }
	}
}
