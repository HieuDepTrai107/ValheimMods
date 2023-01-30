using Jotunn.Managers;
using UnityEngine;

namespace EnhancedBosses.Bosses
{
    public class YagluthTaunt : CustomAttack
    {
        public YagluthTaunt()
        {
            name = "GoblinKing_Taunt";
            bossName = "GoblinKing";
            stopOriginalAttack = false;
        }
    }
}
