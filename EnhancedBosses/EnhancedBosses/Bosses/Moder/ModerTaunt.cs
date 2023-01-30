using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class ModerTaunt : SummonAttack
    {
        public ModerTaunt()
        {
            name = "dragon_taunt";
            bossName = "Dragon";
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
