
namespace UncorRTDPS.Util
{
    public static class UniqueLongGenerator
    {
        private static long id = 0;
        private static object locker = new object();

        public static long GetUniqueId()
        {
            long res;
            lock (locker)
            {
                id += 1;
                res = id;
            }
            return res;
        }
    }
}
