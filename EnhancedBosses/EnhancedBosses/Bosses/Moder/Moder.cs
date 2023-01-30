using Jotunn.Managers;
using Log = Jotunn.Logger;

namespace EnhancedBosses.Bosses
{
    public class Moder : Boss
    {
        public Moder()
        {
            bossName = "Dragon";
            customAttacks = new()
            {
                new ModerBite(),
                new ModerClawLeft(),
                new ModerClawRight(),
                new ModerColdbreath(),
                new ModerTaunt(),
                new ModerSpitShotgun(),
                new ModerSummon()
            };
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Moder>();
        }


        public void Update()
        {
            if (character != null && baseAI != null)
            {
                if (character.GetHealthPercentage() > Main.ModerHealthThreshold.Value)
                {
                    baseAI.m_chanceToLand = 0f;
                }
                else
                {
                    baseAI.m_chanceToLand = 1f;
                    baseAI.m_chanceToTakeoff = 0f;
                    baseAI.m_randomFlyTimer = baseAI.m_airDuration + 1;
                }
            }
        }
    }
}
