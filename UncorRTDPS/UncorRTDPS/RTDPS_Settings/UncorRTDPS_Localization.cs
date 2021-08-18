using System.Collections.Generic;
using System.IO;

namespace UncorRTDPS.RTDPS_Settings
{
    public static class UncorRTDPS_Localization
    {
        private static Dictionary<string, string> dictGUI;

        public static void LoadLocalizationGUI()
        {
            string lang = "en";
            switch (UncorRTDPS_StaticSettings.Language_UI)
            {
                case UncorRTDPS_StaticSettings.Languages.Russian:
                    lang = "ru";
                    break;
                case UncorRTDPS_StaticSettings.Languages.English:
                    lang = "en";
                    break;
            }
            string locFile = Path.GetFullPath(Path.Combine(UncorRTDPS_StaticSettings.ResourcesPath, "locale", lang, "locGUI_rtdps.txt"));
            StreamReader file = new StreamReader(locFile);

            string line;
            string[] d;
            dictGUI = new Dictionary<string, string>();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().Length < 1)
                    continue;
                d = GetKeyAndValue(line);
                dictGUI.Add(d[0], d[1]);
            }
            file.Close();
        }

        private static char keyAndValueDelimiter = '=';
        private static char startEndValueChar = '"';
        private static string[] GetKeyAndValue(string line)
        {
            string[] res = new string[2];

            int keyLength = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i].Equals(keyAndValueDelimiter))
                {
                    keyLength = i;
                    break;
                }
            }

            int posStartOfValue = keyLength + 1;
            for (int i = keyLength; i < line.Length; i++)
            {
                if (line[i].Equals(startEndValueChar))
                {
                    posStartOfValue = i + 1;
                    break;
                }
            }

            int endOfValue = line.Length - 1;
            for (int i = line.Length - 1; i >= posStartOfValue; i--)
            {
                if (line[i].Equals(startEndValueChar))
                {
                    endOfValue = i;
                    break;
                }
            }

            res[0] = line.Substring(0, keyLength);
            res[1] = line.Substring(posStartOfValue, endOfValue - posStartOfValue);

            return res;
        }

        public static string GetLocaleGuiVal(string key)
        {
            string val;
            if (dictGUI.TryGetValue(key, out val))
            {
                return dictGUI[key];
            } else
            {
                return key;
            }
        }
    }
}
