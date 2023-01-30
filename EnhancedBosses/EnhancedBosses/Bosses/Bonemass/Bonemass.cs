using EnhancedBosses.StatusEffects;
using Jotunn.Managers;
using UnityEngine;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Bonemass : Boss
    {
        public Bonemass()
        {
            bossName = "Bonemass";
            customAttacks = new()
            {
                new BonemassPunch(),
                new BonemassAOE(),
                new BonemassThrow(),
                new BonemassSummon()
            };
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Bonemass>();
        }

        public override void Awake()
        {
            base.Awake();
            InvokeRepeating("UpdateAura", 0f, 0.5f);
        }

        public void UpdateAura()
        {
            if (character.m_nview.IsOwner())
            {
                foreach (var player in Helpers.FindPlayers(character.transform.position, 10f))
                {
                    SE_Trip statusEffect1 = ScriptableObject.CreateInstance<SE_Trip>();
                    player.GetSEMan().AddStatusEffect(statusEffect1, true);

                    SE_Slow statusEffect2 = ScriptableObject.CreateInstance<SE_Slow>();
                    player.GetSEMan().AddStatusEffect(statusEffect2, true);
                }
            }
        }
    }
}
