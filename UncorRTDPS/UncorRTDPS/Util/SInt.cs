using System.Globalization;

namespace UncorRTDPS.Util
{
    public static class SInt
    {
        public static int? FromString(string s)
        {
            int res;
            if (int.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out res))
            {
                return res;
            }
            return null;
        }

        public static string ToString(int i)
        {
            return i.ToString(CultureInfo.InvariantCulture);
        }
    }
}
