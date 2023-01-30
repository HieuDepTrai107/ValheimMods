using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ElderStomp : CustomAttack
    {
        public ElderStomp()
        {
            name = "gd_king_stomp";
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
