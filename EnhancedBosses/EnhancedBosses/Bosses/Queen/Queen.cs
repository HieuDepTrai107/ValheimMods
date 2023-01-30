using Jotunn.Managers;

namespace EnhancedBosses.Bosses
{
    public class Queen : Boss
    {
        public Queen()
        {
            bossName = "SeekerQueen";
            customAttacks = new()
            {
                new QueenTeleport(),
                new QueenRush(),
                new QueenBite(),
                new QueenCall(),
                new QueenSpit(),
                new QueenSlap(),
                new QueenPierceAOE()
            };
        }

        public override void SetupCharacter()
        {
            PrefabManager.Instance.GetPrefab(bossName).AddComponent<Queen>();
        }
    }
}
