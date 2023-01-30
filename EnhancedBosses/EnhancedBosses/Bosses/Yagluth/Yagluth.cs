using Jotunn.Managers;

namespace EnhancedBosses.Bosses
{
    public class Yagluth : Boss
    {
        public Yagluth()
        {
            bossName = "GoblinKing";
            customAttacks = new()
            {
                new YagluthBeam(),
                new YagluthNova(),
                new YagluthMeteors(),
                new YagluthTaunt(),
                new YagluthSummon()
            };
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Yagluth>();
        }
    }
}
