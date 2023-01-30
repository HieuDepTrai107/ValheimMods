using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ElderRootSpawn : CustomAttack
    {
        public ElderRootSpawn()
        {
            name = "gd_king_shoot";
            bossName = "gd_king";
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
