using System;
using System.Globalization;

namespace UncorRTDPS.Util
{
    public static class SFloat
    {
        public static float FromString(string s)
        {
            float res;
            if (float.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
            return float.NaN;
        }

        public static string ToString(float f)
        {
            return f.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToString(float f, string format)
        {
            return String.Format(CultureInfo.InvariantCulture, format, f);
        }
    }
}
