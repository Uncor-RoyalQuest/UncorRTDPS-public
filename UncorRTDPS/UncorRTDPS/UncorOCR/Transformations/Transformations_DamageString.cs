using System.Text.RegularExpressions;

namespace UncorRTDPS.UncorOCR.Transformations
{
    public static class Transformations_DamageString
    {
        public static Regex reg_Symbols = new Regex(@"\^|\`|\~|\,|\.|\:|\;|\'|\*|\ |a|а|\$|""|\!");
        public static MatchEvaluator eval_Symbols = match =>
        {
            switch (match.Value)
            {
                case "^": return "";
                case "`": return "";
                case "~": return "";
                case ",": return "";
                case ".": return "";
                case ":": return "";
                case ";": return "";
                case "'": return "";
                case "*": return "";
                case " ": return "";
                case "a": return "4"; //en "a"
                case "а": return "4"; //ru "a"
                case "$": return "6";
                case "\"": return "";
                case "!": return "1";
                default: return "";
            }
        };

        /// <summary>
        /// Faster by regex
        /// </summary>
        /// <param name="rawDamageString"></param>
        /// <returns></returns>
        public static string RemoveGarbageFromRawOcrDamageString_Regex(string rawDamageString)
        {
            string res = reg_Symbols.Replace(rawDamageString, eval_Symbols);
            return res;
        }
    }
}
