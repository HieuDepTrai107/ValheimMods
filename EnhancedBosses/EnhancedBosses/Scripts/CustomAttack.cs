using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses
{
    public class CustomAttack
    {
        public virtual string bossName { get; set; }

        public virtual string name { get; set; }

        public virtual bool stopOriginalAttack { get; set; }

        public virtual float heal { get; set; }


        public virtual void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {

        }


        public float GetHeal()
        {
            return Main.cfg[bossName][name].Heal;
        }

        public float GetHpThreshold()
        {
            return Main.cfg[bossName][name].HpThreshold == 0 ? 1f : Main.cfg[bossName][name].HpThreshold;
        }

        public float GetDistance()
        {
            return Main.cfg[bossName][name].Distance;
        }

        public float GetShieldHp()
        {
            return Main.cfg[bossName][name].ShieldHp;
        }

        public float GetShieldDuration()
        {
            return Main.cfg[bossName][name].ShieldDuration;
        }

        public virtual bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return true;
        }

        public virtual GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();

            return gameObject;
        }
    }
}
