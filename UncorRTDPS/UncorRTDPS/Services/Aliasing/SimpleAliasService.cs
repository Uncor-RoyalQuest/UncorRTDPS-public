using System.Collections.Generic;
using System.IO;

namespace UncorRTDPS.Services.Aliasing
{
    class SimpleAliasService : Service
    {
        private Dictionary<string, string> aliasesDictionary = new Dictionary<string, string>();
        private const string keyValueSeparator = "=";

        public ServiceResponseStatus InitService(string[] args)
        {
            if (args == null || args.Length < 1)
                return ServiceResponseStatus.FAILED;
            string fileName_aliasesFile = args[0];
            try
            {
                if (File.Exists(fileName_aliasesFile))
                {
                    using (StreamReader file = new StreamReader(fileName_aliasesFile))
                    {
                        string line;
                        string[] d;
                        aliasesDictionary = new Dictionary<string, string>();
                        while ((line = file.ReadLine()) != null)
                        {
                            if (line.Trim().Length < 1)
                                continue;
                            d = line.Split(keyValueSeparator);
                            if (d.Length == 2 && d[0] != null && d[1] != null)
                            {
                                string key = d[0].Trim();
                                string val = d[1].Trim();
                                if (key.Length > 0 && val.Length > 0)
                                {
                                    aliasesDictionary[key] = val;
                                }
                            }
                        }

                    }
                }
            }
            catch
            {
                return ServiceResponseStatus.FAILED;
            }
            return ServiceResponseStatus.OK;
        }

        public ServiceResponseStatus CloseService()
        {
            return ServiceResponseStatus.OK;
        }

        public string GetAliasForName(string name)
        {
            if (aliasesDictionary.ContainsKey(name))
                return aliasesDictionary[name];
            return null;
        }
    }
}
