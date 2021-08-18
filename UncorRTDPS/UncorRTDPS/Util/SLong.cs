using System.Globalization;

namespace UncorRTDPS.Util
{
    public static class SLong
    {
        public static long? FromString(string s)
        {
            long res;
            if (long.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
            return null;
        }

        public static string ToString(long l)
        {
            return l.ToString(CultureInfo.InvariantCulture);
        }
    }
}
