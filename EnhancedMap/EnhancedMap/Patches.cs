using HarmonyLib;
using UnityEngine;

namespace EnhancedMap
{
    class Patches
	{
		[HarmonyPatch(typeof(MonsterAI), nameof(MonsterAI.MakeTame))]
		public static class MonsterAI_MakeTame_Prefix
		{
			public static void Postfix(MonsterAI __instance)
			{
				__instance.gameObject.AddComponent<Friendly>();
			}
		}

		[HarmonyPatch(typeof(Character), nameof(Character.Awake))]
		public static class Character_Awake_Postfix
		{
			public static void Postfix(Character __instance)
			{
				if (Main.FriendlyDetection.Value)
                {
					if (Helpers.IsFriendly(__instance) || __instance.IsTamed())
					{
						__instance.gameObject.AddComponent<Friendly>();
					}
                }

				if (Main.BossDetection.Value)
                {
					if (__instance.IsBoss())
					{
						__instance.gameObject.AddComponent<Boss>();
					}
                }
			}
		}

		[HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdateEventPin))]
		public static class UpdateEventMobMinimapPinsPatch
		{
			public static void Postfix()
			{
				if (Main.FriendlyDetection.Value)
                {
					PinManager.CheckFriendlyPins();
                }

				if (Main.BossDetection.Value)
				{
					PinManager.CheckBossPins();
				}
			}
		}

		[HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdatePins))]
		public static class MinimapUpdatePins
		{
			public static void Postfix()
			{
				if (Main.FriendlyDetection.Value)
				{
					foreach (Minimap.PinData pinData in PinManager.GetFriendlyPins())
					{
						pinData.m_iconElement.color = Color.green;
					}
				}
			}
		}

		/*
        [HarmonyPatch(typeof(Terminal), nameof(Terminal.TryRunCommand))]
		public static class Terminal_TryRunCommand_Postfix
        {
			public static void Prefix(Terminal __instance, string text)
            {
				if (text == "killall")
				{
					foreach (var character in Character.GetAllCharacters())
                    {
						int i = Main.bosses.FindIndex(x => x.character == character);
						if (i != -1)
                        {
							Main.bosses[i].OnDeath();
							Main.bosses.RemoveAt(i);
                        }

						int j = Main.friendlies.FindIndex(x => x.character == character);
						if (j != -1)
						{
							Main.friendlies[i].OnDeath();
							Main.friendlies.RemoveAt(i);
						}
					}
				}
            }
        }
		*/
	}
}
