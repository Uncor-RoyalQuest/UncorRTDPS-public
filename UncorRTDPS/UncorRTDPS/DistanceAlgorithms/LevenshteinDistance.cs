using System;

namespace UncorRTDPS.DistanceAlgorithms
{
    public static class LevenshteinDistance
    {

        public static int CalcLevenshteinDistance(string source1, string source2) //O(n*m)
        {
            int source1Length = source1.Length;
            int source2Length = source2.Length;

            int[,] matrix = new int[source1Length + 1, source2Length + 1];

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (int i = 0; i <= source1Length; matrix[i, 0] = i++) { }
            for (int j = 0; j <= source2Length; matrix[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (int i = 1; i <= source1Length; i++)
            {
                for (int j = 1; j <= source2Length; j++)
                {
                    int cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix[source1Length, source2Length];
        }

        public static int[,] matrix_Buffered = new int[100, 100];
        public static int CalcLevenshteinDistance_Buffered(string source1, string source2)
        {
            int source1Length = source1.Length;
            int source2Length = source2.Length;

            // First calculation, if one entry is empty return full length
            if (source1Length == 0)
                return source2Length;

            if (source2Length == 0)
                return source1Length;

            // Adjust matrix_Buffered
            if (matrix_Buffered.Length < Math.Max(source1Length, source2Length))
            {
                matrix_Buffered = new int[Math.Max(source1Length, source2Length), Math.Max(source1Length, source2Length)];
            }

            // Initialization of matrix with row size source1Length and columns size source2Length
            for (var i = 0; i <= source1Length; matrix_Buffered[i, 0] = i++) { }
            for (var j = 0; j <= source2Length; matrix_Buffered[0, j] = j++) { }

            // Calculate rows and collumns distances
            for (var i = 1; i <= source1Length; i++)
            {
                for (var j = 1; j <= source2Length; j++)
                {
                    var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                    matrix_Buffered[i, j] = Math.Min(
                        Math.Min(matrix_Buffered[i - 1, j] + 1, matrix_Buffered[i, j - 1] + 1),
                        matrix_Buffered[i - 1, j - 1] + cost);
                }
            }
            // return result
            return matrix_Buffered[source1Length, source2Length];
        }
    }
}
