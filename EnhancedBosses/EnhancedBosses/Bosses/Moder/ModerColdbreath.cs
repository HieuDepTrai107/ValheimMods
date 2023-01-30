using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ModerColdbreath : CustomAttack
    {
        public ModerColdbreath()
        {
            name = "dragon_coldbreath";
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
