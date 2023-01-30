using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class BonemassSummon : SummonAttack
    {
        public BonemassSummon()
        {
            name = "bonemass_summon";
            bossName = "Bonemass";
            stopOriginalAttack = true;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "bonemass_attack_aoe");

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
