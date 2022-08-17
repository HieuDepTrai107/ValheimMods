using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EnhancedBosses.StatusEffects
{
	class SE_Shielded : SE_Stats
	{
		public float baseTTL = Main.ElderShieldDuration.Value;
		public float hpValue = Main.ElderShieldHP.Value;

		public GameObject shieldFX;
		public Transform _transform;

		public override void Setup(Character character)
		{
			m_ttl = baseTTL;
			SpawnShield();
			base.Setup(character);
		}

		public override void OnDamaged(HitData hit, Character attacker)
		{
			if (hit != null && attacker != null)
			{
				if (hit.GetType() != null)
				{
					float totalDamage = hit.GetTotalDamage();
					hpValue -= totalDamage;
					hit.ApplyModifier(0f);
					Helpers.PlayEffect("fx_GoblinShieldHit", m_character.GetCenterPoint());

					if (hpValue < 0)
					{
						DestroyShield();
						m_time = baseTTL;
					}
				}
			}
		}

        public override void OnDestroy()
        {
			DestroyShield();
            base.OnDestroy();
        }

        public override bool IsDone()
        {
			if (shieldFX != null)
			{
				var centerPoint = m_character.GetCenterPoint();
				var position = new Vector3(centerPoint.x, centerPoint.y + 1f, centerPoint.z);
				_transform.position = position;

				if (m_time > m_ttl)
				{
					DestroyShield();
				}
			}

			return base.IsDone();
        }

		public void SpawnShield()
		{
			var centerPoint = m_character.GetCenterPoint();
			var position = new Vector3(centerPoint.x, centerPoint.y + 1f, centerPoint.z);
			shieldFX = Object.Instantiate<GameObject>(PrefabManager.Instance.GetPrefab("vfx_GoblinShield"), position, Quaternion.identity);
			_transform = shieldFX.transform;
			_transform.position = position;
			_transform.localScale = new Vector3(5f, 5f, 5f);

			foreach (ParticleSystem particleSystem in shieldFX.GetComponentsInChildren<ParticleSystem>())
			{
				particleSystem.Play();
			}
		}

		public void DestroyShield()
        {
			if (shieldFX != null)
            {
				ZNetView component = shieldFX.GetComponent<ZNetView>();
				ZNetScene.instance.m_instances.Remove(component.GetZDO());
				component.Destroy();
				shieldFX = null;
				_transform = null;
				Helpers.PlayEffect("fx_GoblinShieldBreak", m_character.GetCenterPoint());
            }
		}
	}
}
