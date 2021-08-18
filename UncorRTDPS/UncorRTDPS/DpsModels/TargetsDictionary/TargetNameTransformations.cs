using System.Text.RegularExpressions;

namespace UncorRTDPS.DpsModels.TargetsDictionary
{
    public static class TargetNameTransformations
    {

        private static Regex regex = new Regex(@"\^|\`|\~|\,|\.|\:|\;|\'|\*|\ |""");
        /*
        private static MatchEvaluator eval_Symbols = match =>
        {
            switch (match.Value)
            {
                case " ^ ": return "";
                case "`": return "";
                case "~": return "";
                case ",": return "";
                case ".": return "";
                case ":": return "";
                case ";": return "";
                case "'": return "";
                case "*": return "";
                case " ": return "";
                case "\"": return "";
                default: return "";
            }
        };
        */

        public static MatchEvaluator eval_Symbols_EmptyRes = match =>
        {
            return "";
        };

        /// <summary>
        /// Faster by regex
        /// </summary>
        /// <param name="originalTargetName"></param>
        /// <returns></returns>
        public static string MakeTargetNameSearchFriendly_Regex(string originalTargetName)
        {
            string res = regex.Replace(originalTargetName, eval_Symbols_EmptyRes);
            res = res.ToLower();
            return res;
        }
    }
}
