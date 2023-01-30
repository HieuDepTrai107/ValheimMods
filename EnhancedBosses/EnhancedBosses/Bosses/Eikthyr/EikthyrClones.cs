using Jotunn.Entities;
using Jotunn.Managers;
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class EikthyrClones : CustomAttack
    {
        public int count = 3;

        public float radius = 10f;

        public float lifetime = 8f;

        public EikthyrClones()
        {
            name = "Eikthyr_clones";
            bossName = "Eikthyr";
            stopOriginalAttack = true;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return character.GetHealthPercentage() <= GetHpThreshold();
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            Vector3 position = character.transform.position;

            for (int i = 0; i < count; i++)
            {
                Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
                SpawnClone(character, vector);
            }

            Vector3 newPosition = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));
            character.transform.position = newPosition;

            Eikthyr.Thunder(character, newPosition);

            character.Heal(GetHeal());
        }

        public void SpawnClone(Character character, Vector3 position)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab("RRRM_EikthyrClone");
            prefab.GetComponent<Eikthyr>().enabled = false;

            GameObject gameobject = Object.Instantiate(prefab, position, Quaternion.identity);
            Character ch = gameobject.GetComponent<Character>();
            ch.m_faction = Character.Faction.Boss;

            DateTime time = ZNet.instance.GetTime();
            ch.m_nview.GetZDO().Set("spawn_time", time.Ticks);

            var td = gameobject.AddComponent<CharacterTimedDestruction>();
            if (td != null)
            {
                td.m_timeoutMin = lifetime;
                td.m_timeoutMax = lifetime;
                td.Trigger();
            }

            ch.m_baseAI.SetHuntPlayer(true);

            Eikthyr.Thunder(character, position);
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "Eikthyr_stomp");
            
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
