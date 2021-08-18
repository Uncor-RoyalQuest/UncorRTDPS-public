

namespace UncorRTDPS.DpsModels
{
    public class ModelSettings
    {
        public long BossDamageSeparationDelayMS { get; set; } = 30000;
        public long EliteDamageSeparationDelayMS { get; set; } = 15000;
        public long CommonDamageSeparationDelayMS { get; set; } = 3000; //ms


        public ModelSettings(long bossSepDelay, long eliteSepDelay, long commonSepDelay)
        {
            BossDamageSeparationDelayMS = bossSepDelay;
            EliteDamageSeparationDelayMS = eliteSepDelay;
            CommonDamageSeparationDelayMS = commonSepDelay;
        }

        public ModelSettings(ModelSettings s)
        {
            BossDamageSeparationDelayMS = s.BossDamageSeparationDelayMS;
            EliteDamageSeparationDelayMS = s.EliteDamageSeparationDelayMS;
            CommonDamageSeparationDelayMS = s.CommonDamageSeparationDelayMS;
        }

        public static bool IsEquals(ModelSettings s1, ModelSettings s2)
        {
            if (s1.BossDamageSeparationDelayMS == s2.BossDamageSeparationDelayMS &&
                s1.EliteDamageSeparationDelayMS == s2.EliteDamageSeparationDelayMS &&
                s1.CommonDamageSeparationDelayMS == s2.CommonDamageSeparationDelayMS)
                return true;
            return false;
        }
    }
}
