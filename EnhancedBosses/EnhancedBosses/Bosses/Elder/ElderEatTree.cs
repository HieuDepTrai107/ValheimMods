using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    class ElderEatTree : CustomAttack
    {
        float searchDistance = 20f;

        public ElderEatTree()
        {
            name = "gd_king_heal";
            bossName = "gd_king";
            stopOriginalAttack = true;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return CanEatTree(character);
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            EatTree(character);
        }

        public void EatTree(Character character)
        {
            var tree = Elder.FindNearTree(character.transform.position, searchDistance);
            if (tree != null)
            {
                character.Heal(GetHeal());
                Elder.DestroyTree(tree);
                return;
            }

            var log = Elder.FindNearLog(character.transform.position, searchDistance);
            if (log != null)
            {
                character.Heal(0.5f * GetHeal());
                log.Destroy();
                return;
            }
        }

        public bool CanEatTree(Character character)
        {
            if (character.GetHealthPercentage() <= GetHpThreshold())
            {
                return Elder.IsTreeExistInRange(character.transform.position, searchDistance) || Elder.IsLogExistInRange(character.transform.position, searchDistance);
            }

            return false;
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
