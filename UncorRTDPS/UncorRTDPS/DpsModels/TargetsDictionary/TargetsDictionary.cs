using System;
using System.Collections.Generic;
using System.IO;
using UncorRTDPS.Util;

namespace UncorRTDPS.DpsModels.TargetsDictionary
{
    public class TargetsDictionary
    {
        public static List<Target> BossTargets { get; set; }
        public static List<Target> EliteTargets { get; set; }

        private static double acceptableBorderDistance_Lev = 0.2;
        private static double acceptableBorderDistance_Ham = 0.2;

        /// <summary>
        /// Keys = searchFriendlyOriginalName
        /// </summary>
        private static Dictionary<string, Target> simpleDictionary = new Dictionary<string, Target>();

        private static Dictionary<int, List<Target>> targetsBySearchFriendlyNameLength_Dict = new Dictionary<int, List<Target>>();

        public static void AddTargetToSimpleDictionary(Target t)
        {
            if (simpleDictionary.ContainsKey(t.searchFriendlyOriginalName))
                return;

            simpleDictionary.Add(t.searchFriendlyOriginalName, t);
        }

        public static Target TryGetTargetFromSimpleDictionary(string searchFriendlyOriginalName)
        {
            Target t;
            if (simpleDictionary.TryGetValue(searchFriendlyOriginalName, out t))
                return t;
            return null;
        }

        public static void AddTargetToHammingDictionary(Target t)
        {
            int tSearchLen = t.searchFriendlyOriginalName.Length;
            List<Target> targetsOfTheSameLength;
            if (!targetsBySearchFriendlyNameLength_Dict.TryGetValue(tSearchLen, out targetsOfTheSameLength))
            {
                targetsOfTheSameLength = new List<Target>();
                targetsBySearchFriendlyNameLength_Dict.Add(tSearchLen, targetsOfTheSameLength);
            }
            targetsOfTheSameLength.Add(t);
        }

        public static Target TryGetTargetHammingMethodSearch(string searchFriendlyOriginalName)
        {
            int tSearchLen = searchFriendlyOriginalName.Length;
            if (tSearchLen == 0)
                return null;

            List<Target> targetsOfTheSameLength;

            //check if there smth of this length, else assume it is not in the dictionary
            if (!targetsBySearchFriendlyNameLength_Dict.TryGetValue(tSearchLen, out targetsOfTheSameLength))
                return null;

            Target targetMinDistance = null;
            int minHammingDistance = Int32.MaxValue;
            int targetsOfTheSameLengthCount = targetsOfTheSameLength.Count;
            for (int i = 0; i < targetsOfTheSameLengthCount; i++)
            {
                int dist_i = DistanceAlgorithms.HammingDistance.CalcHammingDistance(targetsOfTheSameLength[i].searchFriendlyOriginalName, searchFriendlyOriginalName);
                if (minHammingDistance > dist_i)
                {
                    minHammingDistance = dist_i;
                    targetMinDistance = targetsOfTheSameLength[i];
                }
            }


            // the greater the distance the worse
            double minDistanceProportion = (double)minHammingDistance / tSearchLen;
            if (minDistanceProportion < acceptableBorderDistance_Ham)
                return targetMinDistance;
            else
                return null;
        }

        public static Target TryGetTargetLevenshteinMethodSearch(string searchFriendlyOriginalName)
        {
            int tSearchLen = searchFriendlyOriginalName.Length;
            if (tSearchLen == 0)
                return null;


            //try get levenshtein distance from dictionary with words legnth: Len-1, Len, Len+1
            Target[] targetsMin = { null, null, null };
            int[] distMin = { Int32.MaxValue, Int32.MaxValue, Int32.MaxValue };
            int posTargets = 0;
            for (int i = tSearchLen - 1; i < tSearchLen + 1; i++)
            {
                List<Target> targets;

                //get list of names
                if (!targetsBySearchFriendlyNameLength_Dict.TryGetValue(tSearchLen, out targets))
                {
                    posTargets += 1;
                    continue;
                }

                //find min distance from the list
                int targetsCount = targets.Count;
                for (int j = 0; j < targetsCount; j++)
                {
                    int dist_j = DistanceAlgorithms.LevenshteinDistance.CalcLevenshteinDistance_Buffered(targets[j].searchFriendlyOriginalName, searchFriendlyOriginalName);
                    if (distMin[posTargets] > dist_j)
                    {
                        distMin[posTargets] = dist_j;
                        targetsMin[posTargets] = targets[j];
                    }
                }

                posTargets += 1;
            }

            // the greater the distance the worse

            //find min distance from minDist arr
            int posMinLevDist = 0;
            int minLevDist = distMin[0];
            for (int i = 1; i < 3; i++)
            {
                if (distMin[i] < minLevDist)
                {
                    minLevDist = distMin[i];
                    posMinLevDist = i;
                }
            }

            double minDistanceProportion = (double)minLevDist / tSearchLen;
            if (minDistanceProportion < acceptableBorderDistance_Lev)
                return targetsMin[posMinLevDist];
            else
                return null;

        }

        public static void LoadDictionary(string executingDir, RTDPS_Settings.UncorRTDPS_StaticSettings.Languages lang)
        {
            // path: /src/rtdps_mobs/bosses_ru.txt | /src/rtdps_mobs/bosses_en.txt
            // path: /src/rtdps_mobs/elites_ru.txt | /src/rtdps_mobs/elites_en.txt

            //prep lang folder
            string langFolder = lang == RTDPS_Settings.UncorRTDPS_StaticSettings.Languages.Russian ? "ru" : "en";

            //prep single dictionary
            if (simpleDictionary == null)
                simpleDictionary = new Dictionary<string, Target>();
            simpleDictionary.Clear();

            //prep boss arr
            if (BossTargets == null)
                BossTargets = new List<Target>();
            BossTargets.Clear();

            //prep dict for hamming
            if (targetsBySearchFriendlyNameLength_Dict == null)
                targetsBySearchFriendlyNameLength_Dict = new Dictionary<int, List<Target>>();
            targetsBySearchFriendlyNameLength_Dict.Clear();


            //load bosses
            StreamReader fileBosses = new StreamReader(Path.GetFullPath(Path.Combine(executingDir, "src", "mobs_rtdps", "bosses_" + langFolder + ".txt")));
            string rowBoss;
            int id = 0;
            while ((rowBoss = fileBosses.ReadLine()) != null)
            {
                rowBoss = rowBoss.Trim();
                if (rowBoss.Length < 1)
                    continue;

                (long, string) hpAndName = TryGetHpAndNameFromStringTarget(rowBoss);

                Target target = new Target(id, TargetType.Boss, hpAndName.Item2, hpAndName.Item1);
                BossTargets.Add(target);
                AddTargetToSimpleDictionary(target);
                AddTargetToHammingDictionary(target);
                id += 1;
            }
            fileBosses.Close();


            //prep elite arr
            if (EliteTargets == null)
                EliteTargets = new List<Target>();
            EliteTargets.Clear();

            //load elites
            StreamReader fileElites = new StreamReader(Path.GetFullPath(Path.Combine(executingDir, "src", "mobs_rtdps", "elites_" + langFolder + ".txt")));
            string rowElite;
            id = 0;
            while ((rowElite = fileElites.ReadLine()) != null)
            {
                rowElite = rowElite.Trim();
                if (rowElite.Length < 1)
                    continue;

                (long, string) hpAndName = TryGetHpAndNameFromStringTarget(rowElite);

                Target target = new Target(id, TargetType.Elite, hpAndName.Item2, hpAndName.Item1);
                EliteTargets.Add(target);
                AddTargetToSimpleDictionary(target);
                AddTargetToHammingDictionary(target);
                id += 1;
            }
            fileElites.Close();

            //acceptable distance

            //lev
            double d = SDouble.FromString(RTDPS_Settings.UncorRTDPS_Config.GetConfigVal("similarityBarrier_Lev"));
            if (!Double.IsNaN(d) && d != 0.0d)
            {
                acceptableBorderDistance_Lev = d;
            }

            //ham
            d = SDouble.FromString(RTDPS_Settings.UncorRTDPS_Config.GetConfigVal("similarityBarrier_Ham"));
            if (!Double.IsNaN(d) && d != 0.0d)
            {
                acceptableBorderDistance_Ham = d;
            }
        }

        public static (long, string) TryGetHpAndNameFromStringTarget(string s)
        {
            int posWhiteSpace = -1;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ' ')
                {
                    posWhiteSpace = i;
                    break;
                }
            }

            if (posWhiteSpace != -1)
            {
                long? hp = SLong.FromString(s.Substring(0, posWhiteSpace));
                if (hp != null)
                {
                    return (hp.Value, s.Substring(posWhiteSpace).Trim());
                }
                else
                {
                    return (-1, s);
                }
            }
            else
            {
                return (-1, s);
            }
        }
    }
}
