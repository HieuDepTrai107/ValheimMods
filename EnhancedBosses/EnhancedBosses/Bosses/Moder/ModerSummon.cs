using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class ModerSummon : SummonAttack
    {
        public ModerSummon()
        {
            name = "dragon_summon";
            bossName = "Dragon";
            stopOriginalAttack = true;
        }



        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "dragon_spit_shotgun");
            
            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
