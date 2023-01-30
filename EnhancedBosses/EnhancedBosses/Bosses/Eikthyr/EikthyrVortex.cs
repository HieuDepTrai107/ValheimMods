using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class EikthyrVortex : CustomAttack
    {
        public EikthyrVortex()
        {
            name = "Eikthyr_vortex";
            bossName = "Eikthyr";
            stopOriginalAttack = true;
        }

        public override void OnAttackTriggered(Character character, MonsterAI monsterAI)
        {
            Vector3 vector = character.transform.position + character.transform.forward * 4f + Vector3.up;
            GameObject prefab = PrefabManager.Instance.GetPrefab("ArcaneOrbitSphere");

            TimedDestruction td = prefab.GetComponent<TimedDestruction>();
            td.m_timeout = 3f;

            GameObject gameObject = Object.Instantiate(prefab, vector, Quaternion.identity);

            Vortex vortex = gameObject.GetComponent<Vortex>();
            vortex.character = character;
        }

        public override bool CanUseAttack(Character character, MonsterAI monsterAI)
        {
            return Player.IsPlayerInRange(character.transform.position, 10f);
        }

        public override GameObject Setup()
        {
            GameObject gameObject = PrefabManager.Instance.CreateClonedPrefab(name, "Eikthyr_charge");
            gameObject.name = name;

            ItemDrop item = gameObject.GetComponent<ItemDrop>();
            item.m_itemData.m_shared.m_name = name;

            ItemManager.Instance.AddItem(new CustomItem(gameObject, false));

            return gameObject;
        }
    }
}
