using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class BonemassThrow : CustomAttack
    {
        public BonemassThrow()
        {
            name = "bonemass_attack_throw";
            bossName = "Bonemass";
            stopOriginalAttack = false;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();

            return gameObject;
        }
    }
}
