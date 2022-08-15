using System.Collections.Generic;
using Terraheim.ArmorEffects;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedMap
{
    class Tree : MonoBehaviour
    {
		public Minimap.PinData pin;
		public Sprite sprite;

		public Dictionary<string, Sprite> sprites = new()
		{
			{ "Birch1(Clone)", Main.FineTreeSprite },
			{ "Birch2(Clone)", Main.FineTreeSprite },
			{ "Birch1_aut(Clone)", Main.FineTreeSprite },
			{ "Birch2_aut(Clone)", Main.FineTreeSprite }
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
				if (Player.m_localPlayer.GetSEMan().HaveStatusEffect("Tree Damage Bonus"))
				{
					SE_TreeDamageBonus statusEffect = Player.m_localPlayer.GetSEMan().GetStatusEffect("Tree Damage Bonus") as SE_TreeDamageBonus;

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
