using System.Diagnostics;
using UncorRTDPS.DistanceAlgorithms;
using UncorRTDPS.DpsModels.TargetsDictionary;

namespace UncorRTDPS
{
    public static class Test
    {
        
        public static void TestDistanceAlgorithms()
        {
            Trace.WriteLine("-----Distance algs----");

            string nameOriginal = "Мишень";
            string[] nameToBeTested = { 
                "Мишень.", 
                "Мишнь",
                "Ми.шень",
                ": Мишень",
                ": Мишень.",
                ": Ми*шень.",
                ": Ми--шень.",
                ": Ми--шен.",
                "Ми ши\" нь"
            };

            foreach (string name in nameToBeTested)
            {
                Trace.WriteLine("");
                DistanceTest(nameOriginal, name);
            }

        }

        public static void DistanceTest(string s1, string s2)
        {
            Trace.WriteLine("[" + s1 + "] = [" + TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s1) + "]");
            Trace.WriteLine("[" + s2 + "] = [" + TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s2) + "]");
            Trace.WriteLine("Hamm dist = " + HammingDistance.CalcHammingDistance(
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s1),
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s2)));
            Trace.WriteLine("Lev dist = " + LevenshteinDistance.CalcLevenshteinDistance(
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s1),
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s2)));
            Trace.WriteLine("Lev dist (buffered) = " + LevenshteinDistance.CalcLevenshteinDistance_Buffered(
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s1),
                TargetNameTransformations.MakeTargetNameSearchFriendly_Regex(s2)));
        }
    }
}
