using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ModerClawRight : CustomAttack
    {
        public ModerClawRight()
        {
            name = "dragon_claw_right";
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