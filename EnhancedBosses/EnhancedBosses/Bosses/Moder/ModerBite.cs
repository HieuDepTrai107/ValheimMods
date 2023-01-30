using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ModerBite : CustomAttack
    {
        public ModerBite()
        {
            name = "dragon_bite";
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
