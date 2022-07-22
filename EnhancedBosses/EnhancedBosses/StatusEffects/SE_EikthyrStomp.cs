namespace EnhancedBosses.StatusEffects
{
    class SE_EikthyrStomp : SE_Stats
    {
        public static float m_baseTTL = 3f;
        public float speedAmount = 0.1f;

        public SE_EikthyrStomp()
        {
            name = "EB_EikthyrStomp";
            m_ttl = m_baseTTL;
        }

        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            speed *= speedAmount;
            base.ModifySpeed(baseSpeed, ref speed);
        }
    }
}
