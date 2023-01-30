using EnhancedBosses.StatusEffects;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    class ElderShield : CustomAttack
    {
        public ElderShield()
        {
            name = "gd_king_shield";
            bossName = "gd_king";
            stopOriginalAttack = true;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return (character.GetHealthPercentage() < GetHpThreshold()) && !character.HaveShield();
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            SetupShield(character);
        }

        public void SetupShield(Character character)
        {
            SE_BaseShield shield = ScriptableObject.CreateInstance<SE_BaseShield>();
            shield.m_character = character;
            shield.SetMaxHP(GetShieldHp());
            shield.SetDuration(GetShieldDuration());
            character.GetSEMan().AddStatusEffect(shield);
        }


        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "gd_king_rootspawn");

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
