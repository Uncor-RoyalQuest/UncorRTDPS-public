using System.Collections.Generic;

namespace UncorRTDPS.RTDPS_Settings
{
    public static class UncorRTDPS_Config
    {
        private static Dictionary<string, string> configs;

        public static void LoadConfigs()
        {
            System.IO.StreamReader file = new System.IO.StreamReader(UncorRTDPS_StaticSettings.ConfigFileRTDPS);
            string line;
            string[] d;
            configs = new Dictionary<string, string>();
            while ((line = file.ReadLine()) != null)
            {
                if (line.Trim().Length < 1)
                    continue;
                d = GetKeyAndValue(line);
                configs.Add(d[0], d[1]);
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

        public static void SaveConfigs()
        {
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(UncorRTDPS_StaticSettings.ConfigFileRTDPS);
                foreach (KeyValuePair<string, string> entry in configs)
                {
                    sw.WriteLine(entry.Key + "=\"" + entry.Value + "\"");
                }
                sw.Close();
            }
            catch { }
        }

        public static string GetConfigVal(string cfgName)
        {
            if (!configs.ContainsKey(cfgName))
            {
                configs.Add(cfgName, "0");
                SaveConfigs();
            }
            return configs[cfgName];
        }

        public static void UpdateConfigVal(string cfgKey, string newValue)
        {
            configs[cfgKey] = newValue;
        }

        public static bool? GetConfigVal_Bool(string cfgName)
        {
            bool res;
            if (!bool.TryParse(GetConfigVal(cfgName), out res))
            {
                return null;
            }
            return res;
        }
    }
}
