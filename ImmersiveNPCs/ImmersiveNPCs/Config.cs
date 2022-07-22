using System.Collections.Generic;

namespace ImmersiveNPCs
{
	public static class Config
	{
		public static Dictionary<string, float> maxWeight = new Dictionary<string, float>()
		{
			{ "RRRN_Friendly_T1_M(Clone)", Main.maxWeightT1.Value },
			{ "RRRN_Friendly_T1_F(Clone)", Main.maxWeightT1.Value },
			{ "RRRN_Friendly_T2_M(Clone)", Main.maxWeightT2.Value },
			{ "RRRN_Friendly_T2_F(Clone)", Main.maxWeightT2.Value },
			{ "RRRN_Friendly_T3_M(Clone)", Main.maxWeightT3.Value },
			{ "RRRN_Friendly_T3_F(Clone)", Main.maxWeightT3.Value },
			{ "RRRN_Friendly_T4_M(Clone)", Main.maxWeightT4.Value },
			{ "RRRN_Friendly_T4_F(Clone)", Main.maxWeightT4.Value },
			{ "RRRN_Friendly_T5_M(Clone)", Main.maxWeightT5.Value },
			{ "RRRN_Friendly_T5_F(Clone)", Main.maxWeightT5.Value },
			{ "RRRN_Friendly_T6_M(Clone)", Main.maxWeightT6.Value },
			{ "RRRN_Friendly_T6_F(Clone)", Main.maxWeightT6.Value },
		};
	}
}
