using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class ElderSummon : SummonAttack
    {
        public ElderSummon()
        {
            name = "gd_king_summon";
            bossName = "gd_king";
            stopOriginalAttack = true;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "gd_king_rootspawn");

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
