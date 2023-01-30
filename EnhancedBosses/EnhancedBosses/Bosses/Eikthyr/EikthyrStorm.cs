using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class EikthyrStorm : CustomAttack
    {
        public EikthyrStorm()
        {
            name = "Eikthyr_storm";
            bossName = "Eikthyr";
            stopOriginalAttack = true;
        }

        public float count = 10f;

        public float radius = 20f;


        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            Vector3 position = character.transform.position;
            GameObject gameObject = PrefabManager.Instance.GetPrefab("dragonlightningAOEEV");

            for (int i = 0; i < count; i++)
            {
                float xAttitude = Random.Range(-radius, radius);
                float yAttitude = Random.Range(-radius, radius);

                xAttitude = Mathf.Abs(xAttitude) < 3f ? xAttitude < 0 ? -3f : 3f : xAttitude;
                yAttitude = Mathf.Abs(yAttitude) < 3f ? yAttitude < 0 ? -3f : 3f : yAttitude;

                Vector3 vector = new Vector3(position.x + xAttitude, position.y, position.z + yAttitude);
                Object.Instantiate(gameObject, vector, Quaternion.identity);
            }
        }


        public override GameObject Setup()
        {
            GameObject gameObject1 = PrefabManager.Instance.GetPrefab("dragonlightningAOEEV");
            Aoe aoe = gameObject1.GetComponentInChildren<Aoe>();
            // aoe.m_triggerEnterOnly = true;
            aoe.m_hitFriendly = false;
            aoe.m_ttl = 2f;
            aoe.m_damage.m_lightning = 20f;


            GameObject gameObject2 = PrefabManager.Instance.CreateClonedPrefab(name, "Eikthyr_stomp");

            ItemDrop item = gameObject2.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            item.m_itemData.m_shared.m_spawnOnHit = gameObject1;

            ItemManager.Instance.AddItem(new CustomItem(gameObject2, false));

            return gameObject2;
        }
    }
}
