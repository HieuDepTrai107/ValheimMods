using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class EikthyrSummon : SummonAttack
    {
        public EikthyrSummon()
        {
            name = "Eikthyr_summon";
            bossName = "Eikthyr";
            stopOriginalAttack = true;
        }

        public override void SpawnOne(Character character, MonsterAI monsterAI)
        {
            Vector3 position = character.transform.position;
            Vector3 vector = new Vector3(position.x + Random.Range(-radius, radius), position.y, position.z + Random.Range(-radius, radius));

            GameObject go = Object.Instantiate(GetRandomCreature(), vector, Quaternion.identity);
            Character ch = go.GetComponent<Character>();

            if (monsterAI.m_targetCreature != null)
            {
                ch.m_baseAI.SetTargetInfo(monsterAI.m_targetCreature.GetZDOID());
            }

            ch.m_baseAI.SetHuntPlayer(true);
            ch.m_level = Random.Range(1, 3);

            Eikthyr.Thunder(character, vector);
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab("Eikthyr_summon", "Eikthyr_charge");
            
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = "Eikthyr_summon";

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
