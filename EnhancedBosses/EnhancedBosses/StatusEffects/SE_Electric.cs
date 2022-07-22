using Jotunn.Utils;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EnhancedBosses.StatusEffects
{
    class SE_Electric : StatusEffect
	{       
		public GameObject _shockEffect;
		public Transform _transform;
		public float m_baseTTL = 5f;
		public float speedAmount = 0.5f;

		public float m_damage = 3f;
		public float m_damageInterval = 1f;
		public float m_timer;
		public int counter = 5;

		public SE_Electric()
		{
			m_name = "Напряжение";
			name = "EB_Electric";
			m_ttl = m_baseTTL;
			m_icon = AssetUtils.LoadSpriteFromFile("EnhancedBosses/Assets/voltage.png");
			m_flashIcon = true;
		}

		public override void Setup(Character character)
		{
			base.Setup(character);
			CreateShock();
		}

		public override void Stop()
		{
			base.Stop();
			RemoveShock();
		}

        public override void ModifySpeed(float baseSpeed, ref float speed)
		{
			speed *= speedAmount;
			base.ModifySpeed(baseSpeed, ref speed);
        }

        public override void UpdateStatusEffect(float dt)
		{
			base.UpdateStatusEffect(dt);
			if (!m_character.IsTeleporting() && _shockEffect == null)
			{
				CreateShock();
			}
			else
			{
				if (!m_character.IsTeleporting() && _transform != null)
				{
					Tuple<Vector3, Vector3> targetPosition = Utils.GetTargetPosition(m_character);
					_transform.position = targetPosition.Item1;
					_transform.forward = targetPosition.Item2;
				}
				else
				{
					if (m_character.IsDead())
					{
						RemoveShock();
					}
				}
			}

			m_timer -= dt;
			if (m_timer <= 0f && counter > 0)
			{
				m_timer = m_damageInterval;
				HitData hitData = new HitData();
				hitData.m_point = m_character.GetCenterPoint();
				hitData.m_damage.m_lightning = m_damage * Random.Range(0.75f, 1.25f);
				m_character.ApplyDamage(hitData, true, false);
				counter--;
			}
		}

		public void CreateShock(Vector3 target)
		{
			_shockEffect = Object.Instantiate<GameObject>(ZNetScene.instance.GetPrefab("fx_eikthyr_stomp"), target, Quaternion.Euler(0f, 0f, 0f));
			foreach (ParticleSystem particleSystem in _shockEffect.GetComponentsInChildren<ParticleSystem>())
			{
				particleSystem.Play();
			}
		}

		public void CreateShock()
		{
			Tuple<Vector3, Vector3> targetPosition = Utils.GetTargetPosition(m_character);
			if (targetPosition != null)
            {
				_shockEffect = Object.Instantiate<GameObject>(ZNetScene.instance.GetPrefab("fx_Lightning"), targetPosition.Item1, Quaternion.identity);
				_transform = _shockEffect.transform;
				_transform.forward = targetPosition.Item2;
				foreach (ParticleSystem particleSystem in _shockEffect.GetComponentsInChildren<ParticleSystem>())
				{
					particleSystem.Play();
				}
            }
		}

		public void RemoveShock()
		{
			if (_shockEffect != null)
            {
				ZNetScene.instance.m_instances.Remove(_shockEffect.GetComponent<ZNetView>().GetZDO());
				_shockEffect.GetComponent<ZNetView>().Destroy();
				_shockEffect = null;
				_transform = null;
            }
		}
	}
}
