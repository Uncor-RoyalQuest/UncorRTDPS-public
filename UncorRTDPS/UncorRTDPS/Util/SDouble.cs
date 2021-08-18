using System;
using System.Globalization;

namespace UncorRTDPS.Util
{
    public static class SDouble
    {
        public static double FromString(string s)
        {
            double res;
            if (Double.TryParse(s.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
            return Double.NaN;
        }

        public static string ToString(double d)
        {
            return d.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToString(double d, string format)
        {
            return String.Format(CultureInfo.InvariantCulture, format, d);
        }
    }
}
