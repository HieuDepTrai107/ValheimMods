namespace EnhancedBosses.StatusEffects
{
    class SE_Slow : SE_Stats
    {
        public static float m_baseTTL = 1f;
        public float speedAmount = 0.4f;

        public SE_Slow()
        {
            name = "EB_Slow";
            m_ttl = m_baseTTL;
        }

        public override void ModifySpeed(float baseSpeed, ref float speed)
        {
            speed *= speedAmount;
            base.ModifySpeed(baseSpeed, ref speed);
        }
    }
}
