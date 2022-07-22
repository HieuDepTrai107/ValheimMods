using UnityEngine;

namespace EnhancedBosses
{
    class PinManager
    {
		public static Minimap.PinType BossPinType = Minimap.PinType.Boss;

		public static Sprite BossPinSprite = Minimap.instance.GetSprite(Minimap.PinType.Boss);

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
	}
}
