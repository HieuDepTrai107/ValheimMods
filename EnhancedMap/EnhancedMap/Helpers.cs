using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EnhancedMap
{
    class Helpers
    {
        public static bool IsFriendly(Character character)
        {
            return character.name.ToLower().Contains("friendly");
        }

		public static bool IsFriendly(MonsterAI monsterAI)
		{
			return monsterAI.m_character.name.ToLower().Contains("friendly");
		}

		public static List<MonsterAI> GetEnemies(float maxDistance)
		{
			List<MonsterAI> list = new List<MonsterAI>();
			foreach (MonsterAI monsterAI in BaseAI.GetAllInstances().OfType<MonsterAI>())
			{
				if (monsterAI.IsEnemey(Player.m_localPlayer) && !IsFriendly(monsterAI) && !monsterAI.m_character.m_tamed && Vector3.Distance(Player.m_localPlayer.transform.position, monsterAI.transform.position) < maxDistance)
				{
					list.Add(monsterAI);
				}
			}
			return list;
		}
	}
}
