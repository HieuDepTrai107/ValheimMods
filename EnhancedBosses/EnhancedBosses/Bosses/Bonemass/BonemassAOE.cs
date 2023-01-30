using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class BonemassAOE : CustomAttack
    {
        public BonemassAOE()
        {
            name = "bonemass_attack_aoe";
            bossName = "Bonemass";
            stopOriginalAttack = false;
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            AOEDebuffs(character);
        }

        public void AOEDebuffs(Character character)
        {
            foreach (var player in Helpers.FindPlayers(character.transform.position, 15f))
            {
                foreach (ItemDrop.ItemData item in new ItemDrop.ItemData[] { player.m_helmetItem, player.m_chestItem, player.m_legItem, player.m_shoulderItem, player.m_utilityItem })
                {
                    if (item != null && item.m_shared.m_useDurability)
                    {
                        item.m_durability = Mathf.Max(0f, item.m_durability - Random.Range(0.25f, 0.5f) * item.m_durability);
                    }
                }
            }
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.GetPrefab(name);

            ItemDrop item = gameObject.GetComponent<ItemDrop>();

            return gameObject;
        }
    }
}
