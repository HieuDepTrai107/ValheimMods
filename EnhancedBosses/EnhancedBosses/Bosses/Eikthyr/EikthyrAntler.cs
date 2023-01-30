using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class EikthyrAntler : CustomAttack
    {
        public EikthyrAntler()
        {
            name = "Eikthyr_antler";
            bossName = "Eikthyr";
            stopOriginalAttack = false;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return base.CanUseAttack(character, monsterAI);
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            base.OnAttackTriggered(character, monsterAI);
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();

            return gameObject;
        }
    }
}
