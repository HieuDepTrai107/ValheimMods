using UnityEngine;
using Jotunn.Managers;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Eikthyr : Boss
    {
        public int Warp_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "Water", "character", "character_net", "character_ghost");
        public int ScriptChar_Layermask = LayerMask.GetMask("Default", "static_solid", "Default_small", "piece_nonsolid", "terrain", "vehicle", "piece", "viewblock", "character", "character_net", "character_ghost");

        public Eikthyr()
        {
            bossName = "Eikthyr";
            customAttacks = new()
            {
                new EikthyrAntler(),
                new EikthyrCharge(),
                new EikthyrStomp(),
                new EikthyrVortex(),
                new EikthyrSummon()
            };

            if (Helpers.IsRRRCoreInstalled())
            {
                customAttacks.Add(new EikthyrClones());
            }

            if (Helpers.IsMonsterLabzInstalled())
            {
                customAttacks.Add(new EikthyrStorm());
            }
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Eikthyr>();
        }

        public override void Awake()
        {
            base.Awake();
            zNetView.GetZDO().Set("specialAbility", true);
        }

        public void Update()
        {
            if (!Helpers.IsRRRCoreInstalled()) return;

            if (character.GetHealthPercentage() < 0.5f)
            {
                if (zNetView.GetZDO().GetBool("specialAbility"))
                {
                    customAttacks[5].OnAttackTriggered(character, monsterAI);
                    zNetView.GetZDO().Set("specialAbility", false);
                }
            }
        }

        public static void Thunder(Character character, Vector3 position)
        {
            Heightmap.GetHeight(position, out float height);

            Vector3 vector = new Vector3(position.x - 0.5f, height - 5f, position.z + 1f);
            Object.Instantiate(ZNetScene.instance.GetPrefab("fx_eikthyr_forwardshockwave"), vector, Quaternion.Euler(-90f, 0f, 0f));

            HitData hitData = new HitData();
            hitData.m_damage.m_lightning = Random.Range(15f, 25f);
            hitData.SetAttacker(character);

            foreach (var enemy in Helpers.GetEnemiesInRange(character, 2f))
            {
                enemy.Damage(hitData);
            }
        }
    }
}
