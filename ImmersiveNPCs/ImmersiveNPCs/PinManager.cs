using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ImmersiveNPCs
{
	public class PinManager
    {
		public static Minimap.PinType FriendlyPinType = Minimap.PinType.Death;
		public static Sprite FriendlyPinSprite = Minimap.instance.GetSprite(Minimap.PinType.Icon3);

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

		public static IEnumerable<Minimap.PinData> GetFriendlyPins()
        {
			return Minimap.instance.m_pins.Where(x => x.m_type == FriendlyPinType && x.m_icon == FriendlyPinSprite);
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
