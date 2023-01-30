using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jotunn.Managers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Log = Jotunn.Logger;

namespace EnhancedBosses
{
    public class SummonAttack : CustomAttack
    {
        public virtual List<string> creatures { get; set; }

        public override bool stopOriginalAttack => true;

        public float radius = 10f;

        public float searchMinionsRadius = 40f;

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return GetSpawnedCount(character) < CalculateMaxCount();
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            for (int i = 0; i < CalculateSpawnCount(character); i++)
            {
                SpawnOne(character, monsterAI);
            }
        }

        public virtual void SpawnOne(Character character, MonsterAI monsterAI)
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

            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_Potion_stamina_medium"), vector, Quaternion.identity);
            Object.Instantiate(ZNetScene.instance.GetPrefab("vfx_WishbonePing"), vector, Quaternion.identity);
        }

        public List<string> GetCreatures()
        {
            return Main.cfg[bossName][name].Creatures;
        }

        public int GetMaxMinionsCount()
        {
            return Main.cfg[bossName][name].maxMinionsCount;
        }

        public int GetmaxMinionsCountPerPlayer()
        {
            return Main.cfg[bossName][name].maxMinionsCount;
        }

        public int GetSpawnMinionsCount()
        {
            return Main.cfg[bossName][name].spawnMinionsCount;
        }

        public int GetSpawnMinionsCountPerPlayer()
        {
            return Main.cfg[bossName][name].spawnMinionsCountPerPlayer;
        }


        public GameObject GetRandomCreature()
        {
            List<string> creatures = GetCreatures();
            int index = Random.Range(0, creatures.Count - 1);
            return PrefabManager.Instance.GetPrefab(creatures[index]);
        }

        public int CalculateMaxCount()
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            return GetMaxMinionsCount() + (playersCount - 1) * GetmaxMinionsCountPerPlayer();
        }

        public int CalculateSpawnCount(Character character)
        {
            var playersCount = ZNet.instance.GetNrOfPlayers();
            var rawCount = GetSpawnMinionsCount() + (playersCount - 1) * GetSpawnMinionsCountPerPlayer();
            return (int)Mathf.Lerp(0f, rawCount, CalculateMaxCount() - GetSpawnedCount(character));
        }

        public int GetSpawnedCount(Character character)
        {
            int minionsCount = 0;
            foreach (BaseAI baseAI in BaseAI.GetAllInstances())
            {
                if (GetCreatures().Any(e => baseAI.name.Contains(e)))
                {
                    float distance = Utils.DistanceXZ(baseAI.transform.position, character.transform.position);
                    if (distance < searchMinionsRadius)
                    {
                        minionsCount++;
                    }
                }
            }

            return minionsCount;
        }

    }
}
