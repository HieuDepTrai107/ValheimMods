using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ModerClawLeft : CustomAttack
    {
        public ModerClawLeft()
        {
            name = "dragon_claw_left";
            bossName = "Dragon";
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
