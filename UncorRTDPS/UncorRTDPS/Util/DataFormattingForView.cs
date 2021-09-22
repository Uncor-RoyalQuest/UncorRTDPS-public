using System;

namespace UncorRTDPS.Util
{
    public static class DataFormattingForView
    {
        public static string GetPercentFromDamageAndHp(long dmg, long? hp)
        {
            if (hp == null)
                return null;
            return GetPercentFromDamageAndHp(dmg, hp.Value);
        }

        public static string GetPercentFromDamageAndHp(long dmg, long hp)
        {
            if (hp < 1)
                return null;
            return String.Format("{0:0.##}%", ((double)dmg / hp) * 100);
        }
    }
}
