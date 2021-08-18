
namespace UncorRTDPS.DistanceAlgorithms
{
    public static class HammingDistance
    {
        public static int CalcHammingDistance(string s1, string s2)
        {
            if (s1.Length != s2.Length)
                return -1;

            int distance = 0;
            for (int i = 0; i < s1.Length; i++)
                if (s1[i] != s2[i])
                    distance += 1;

            return distance;
        }
    }
}
