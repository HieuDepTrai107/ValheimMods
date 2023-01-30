using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class YagluthBeam : CustomAttack
    {
        public YagluthBeam()
        {
            name = "GoblinKing_Beam";
            bossName = "GoblinKing";
            stopOriginalAttack = false;
        }
    }
}
