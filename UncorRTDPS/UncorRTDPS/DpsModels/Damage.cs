namespace UncorRTDPS.DpsModels
{
    public struct Damage
    {
        public long Dmg;
        public long Time;

        public Damage(long dmg, long time)
        {
            Dmg = dmg;
            Time = time;
        }
    }
}
