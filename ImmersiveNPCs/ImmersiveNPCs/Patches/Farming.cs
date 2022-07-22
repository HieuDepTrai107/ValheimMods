using HarmonyLib;

namespace ImmersiveNPCs.Patches
{
	class Farming
    {

		[HarmonyPatch(typeof(TreeBase), nameof(TreeBase.RPC_Damage))]
		public static class TreeBase_RPC_Damage_Patch
		{
			public static void Postfix(TreeBase __instance, HitData hit)
			{
				if (hit == null) return;
				if (hit.GetAttacker() == null) return;
				if (!hit.GetAttacker().IsPlayer()) return;

				foreach (var friendly in Helpers.GetFriendlies())
				{
					friendly.HitNearTree();
				}
			}
		}

		[HarmonyPatch(typeof(TreeLog), nameof(TreeLog.RPC_Damage))]
		public static class TreeLog_RPC_Damage_Patch
		{
			public static void Postfix(TreeLog __instance, HitData hit)
			{
				if (hit == null) return;
				if (hit.GetAttacker() == null) return;
				if (!hit.GetAttacker().IsPlayer()) return;

				foreach (var friendly in Helpers.GetFriendlies())
				{
					friendly.HitNearTree();
				}
			}
		}

		[HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Damage))]
		public static class MineRock5_Damage_Postfix
		{
			public static void Postfix(HitData hit)
			{
				if (hit == null) return;
				if (hit.GetAttacker() == null) return;
				if (!hit.GetAttacker().IsPlayer()) return;

				foreach (var friendly in Helpers.GetFriendlies())
				{
					friendly.HitNearRock();
				}
			}
		}

		[HarmonyPatch(typeof(MineRock), nameof(MineRock.Damage))]
		public static class MineRock_Damage_Postfix
		{
			public static void Postfix(HitData hit)
			{
				if (hit == null) return;
				if (hit.GetAttacker() == null) return;
				if (!hit.GetAttacker().IsPlayer()) return;

				foreach (var friendly in Helpers.GetFriendlies())
				{
					friendly.HitNearRock();
				}
			}
		}

		[HarmonyPatch(typeof(Destructible), nameof(Destructible.Damage))]
		public static class Destructible_Damage_Postfix
		{
			public static void Postfix(HitData hit)
			{
				if (hit == null) return;
				if (hit.GetAttacker() == null) return;
				if (!hit.GetAttacker().IsPlayer()) return;

				foreach (var friendly in Helpers.GetFriendlies())
				{
					friendly.HitNearRock();
				}
			}
		}
	}
}
