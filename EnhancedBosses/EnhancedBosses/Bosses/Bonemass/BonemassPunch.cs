using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class BonemassPunch : CustomAttack
    {
        public BonemassPunch()
        {
            name = "bonemass_attack_punch";
            bossName = "Bonemass";
            stopOriginalAttack = false;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_attackForce = 300f;

            return gameObject;
        }
    }
}
