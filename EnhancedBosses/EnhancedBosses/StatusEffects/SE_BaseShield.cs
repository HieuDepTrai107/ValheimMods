using UnityEngine;

namespace EnhancedBosses.StatusEffects
{
	public class SE_BaseShield : SE_CustomShield
	{
		public SE_BaseShield()
        {
			name = "SE_BaseShield";
			shieldPrefabName = "vfx_GoblinShield";
			hitEffectPrefabName = "fx_GoblinShieldHit";
			destroyEffectPrefabName = "fx_GoblinShieldBreak";
			shieldScale = 5f;
			shieldSpawnOffset = new Vector3(0f, 1f, 0f);
		}
	}
}
