using Jotunn.Entities;
using Jotunn.Managers;
using System.Collections.Generic;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class ElderDemolishTrees : CustomAttack
    {
        public float searchRadius = 15f;

        public ElderDemolishTrees()
        {
            name = "gd_king_demolish";
            bossName = "gd_king";
            stopOriginalAttack = true;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return CanDemolishTrees(monsterAI);
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            DemolishTrees(monsterAI);
        }

        public bool CanDemolishTrees(MonsterAI monsterAI)
        {
            return monsterAI != null && monsterAI.HaveTarget() && monsterAI.GetTargetCreature() != null && Elder.IsTreeExistInRange(monsterAI.GetTargetCreature().transform.position, searchRadius);
        }

        public void DemolishTrees(MonsterAI monsterAI)
        {
            var trees = Elder.FindNearTrees(monsterAI.GetTargetCreature().transform.position, searchRadius);
            foreach (var tree in trees)
            {
                DemolishTree(monsterAI, tree);
            }
        }

        public void DemolishTree(MonsterAI monsterAI, TreeBase tree)
        {
            tree.m_destroyedEffect.Create(tree.transform.position, tree.transform.rotation, tree.transform);

            var direcion = monsterAI.GetTargetCreature().transform.position - tree.transform.position;
            tree.SpawnLog(direcion);

            List<GameObject> dropList = tree.m_dropWhenDestroyed.GetDropList();
            for (int i = 0; i < dropList.Count; i++)
            {
                Vector2 vector = Random.insideUnitCircle * 0.5f;
                Vector3 position = tree.transform.position + Vector3.up * tree.m_spawnYOffset + new Vector3(vector.x, tree.m_spawnYStep * i, vector.y);
                Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 360), 0f);
                Object.Instantiate(dropList[i], position, rotation);
            }
            tree.gameObject.SetActive(value: false);
            tree.m_nview.Destroy();
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "gd_king_rootspawn");

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;
            item.m_itemData.m_shared.m_aiAttackRange = 50f;
            item.m_itemData.m_shared.m_aiAttackRangeMin = 10f;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
