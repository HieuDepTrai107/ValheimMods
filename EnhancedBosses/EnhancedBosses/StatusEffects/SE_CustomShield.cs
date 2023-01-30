using Jotunn.Managers;
using UnityEngine;
using Object = UnityEngine.Object;
using Log = Jotunn.Logger;

namespace EnhancedBosses
{
    public class SE_CustomShield : SE_Stats
    {
        public float maxHP;
        public float currentHP;
        public float duration;

        public string shieldPrefabName = "";
        public string hitEffectPrefabName = "";
        public string destroyEffectPrefabName = "";
        public float shieldScale = 1f;
        public Vector3 shieldSpawnOffset = new Vector3(0f, 0f, 0f);

        public GameObject shieldFX;

        public override void Setup(Character character)
        {
            m_ttl = duration;
            m_character = character;
            currentHP = maxHP;
            SpawnShield();
            base.Setup(character);
        }

        public override bool IsDone()
        {
            if (shieldFX != null)
            {
                Vector3 centerPoint = m_character.GetCenterPoint();
                Vector3 position = centerPoint + shieldSpawnOffset;
                shieldFX.transform.position = position;

                if (m_time > m_ttl)
                {
                    DestroyShield();
                }
            }

            return base.IsDone();
        }

        public override void OnDamaged(HitData hit, Character attacker)
        {
            if (hit != null && attacker != null)
            {
                if (hit.GetType() != null)
                {
                    float totalDamage = hit.GetTotalDamage();
                    currentHP -= totalDamage;
                    hit.m_damage.Modify(0f);

                    if (hitEffectPrefabName != "")
                    {
                        Object.Instantiate(PrefabManager.Instance.GetPrefab(hitEffectPrefabName), m_character.GetCenterPoint(), Quaternion.identity);
                    }

                    if (currentHP < 0)
                    {
                        DestroyShield();
                        m_time = m_ttl;
                    }
                }
            }

            base.OnDamaged(hit, attacker);
        }

        public void SpawnShield()
        {
            Vector3 centerPoint = m_character.GetCenterPoint();
            Vector3 position = centerPoint + shieldSpawnOffset;
            shieldFX = Object.Instantiate(PrefabManager.Instance.GetPrefab(shieldPrefabName), position, Quaternion.identity);
            shieldFX.transform.localScale = shieldScale * Vector3.one;

            foreach (ParticleSystem particleSystem in shieldFX.GetComponentsInChildren<ParticleSystem>())
            {
                particleSystem.Play();
            }
        }

        public void DestroyShield()
        {
            if (shieldFX != null && m_character != null)
            {
                OnDestroyShield();

                if (destroyEffectPrefabName != "")
                {
                    Object.Instantiate(PrefabManager.Instance.GetPrefab(destroyEffectPrefabName), m_character.GetCenterPoint(), Quaternion.identity);
                }

                ZNetView component = shieldFX.GetComponent<ZNetView>();
                ZNetScene.instance.m_instances.Remove(component.GetZDO());
                component.Destroy();
                shieldFX = null;
            }
        }

        public virtual void OnDestroyShield()
        {
            
        }

        public override void OnDestroy()
        {
            Log.LogWarning("OnDestroy");
            DestroyShield();
            base.OnDestroy();
        }

        public float GetMaxHP()
        {
            return maxHP;
        }

        public void SetMaxHP(float value)
        {
            maxHP = value;
        }

        public float GetCurrentHP()
        {
            return currentHP;
        }

        public void SetCurrentHP(float value)
        {
            currentHP = value;
        }

        public void ResetHP()
        {
            currentHP = maxHP;
        }

        public float GetHealthPercentage()
        {
            return currentHP / maxHP;
        }

        public void SetDuration(float value)
        {
            duration = value;
        }
    }
}
