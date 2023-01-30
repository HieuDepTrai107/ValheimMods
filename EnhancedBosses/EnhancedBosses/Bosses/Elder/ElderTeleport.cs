using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class ElderTeleport : CustomAttack
    {
        public float minTeleportDistance = 15f;

        public ElderTeleport()
        {
            name = "gd_king_teleport";
            bossName = "gd_king";
            stopOriginalAttack = true;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return CanUseTeleport(character, monsterAI);
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            Teleport(character, monsterAI);
        }

        public void Teleport(Character character, MonsterAI monsterAI)
        {
            if (character.m_baseAI.HaveTarget())
            {
                var target = character.m_baseAI.GetTargetCreature();
                if (target.IsPlayer())
                {
                    Vector3 targetPosition = target.transform.position;
                    float distanceToTarget = Vector3.Distance(targetPosition, character.transform.position);
                    if (distanceToTarget > minTeleportDistance)
                    {
                        var tree = Elder.FindNearTree(targetPosition, 15f);
                        if (tree != null)
                        {
                            if (Vector3.Distance(targetPosition, tree.transform.position) < 15f)
                            {
                                character.transform.position = tree.transform.position;
                                character.Heal(heal);
                                Elder.DestroyTree(tree);
                            }
                        }
                    }
                }
            }
        }

        public bool CanUseTeleport(Character character, MonsterAI monsterAI)
        {
            if (character.m_baseAI.HaveTarget())
            {
                var target = monsterAI.GetTargetCreature();
                if (target != null && target.IsPlayer())
                {
                    Vector3 targetPosition = target.transform.position;
                    float distanceToTarget = Vector3.Distance(targetPosition, character.transform.position);
                    if (distanceToTarget > minTeleportDistance)
                    {
                        var tree = Elder.FindNearTree(targetPosition, 15f);
                        if (tree != null)
                        {
                            if (Vector3.Distance(targetPosition, tree.transform.position) < 15f)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "gd_king_rootspawn");

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;
            item.m_itemData.m_shared.m_aiPrioritized = true;
            item.m_itemData.m_shared.m_aiAttackRange = 100f;
            item.m_itemData.m_shared.m_aiAttackRangeMin = 10f;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
