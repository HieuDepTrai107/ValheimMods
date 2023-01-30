using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class YagluthSummon : SummonAttack
    {
        public YagluthSummon()
        {
            name = "GoblinKing_Summon";
            bossName = "GoblinKing";
            stopOriginalAttack = true;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "GoblinKing_Nova");
            
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
