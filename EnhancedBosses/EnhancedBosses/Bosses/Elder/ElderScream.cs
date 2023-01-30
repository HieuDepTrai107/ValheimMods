using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ElderScream : CustomAttack
    {
        public ElderScream()
        {
            name = "gd_king_scream";
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
