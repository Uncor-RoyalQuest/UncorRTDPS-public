using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UncorRTDPS.Services.HotKeys
{
    class HotKeysStorageService : Service
    {

        private Dictionary<string, HotKeyCombination> hotKeyCombinations = new Dictionary<string, HotKeyCombination>();
        private string fileName_jsonDictionaryHotKeyCombinations;

        /// <summary>
        /// args[0] = fileName_jsonDictionaryHotKeyCombinations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ServiceResponseStatus InitService(string[] args)
        {
            if (args == null || args.Length < 1)
                return ServiceResponseStatus.FAILED;
            string fileName_jsonDictionaryHotKeyCombinations = args[0];
            this.fileName_jsonDictionaryHotKeyCombinations = fileName_jsonDictionaryHotKeyCombinations;
            try
            {
                if (File.Exists(fileName_jsonDictionaryHotKeyCombinations))
                {
                    StreamReader file = new StreamReader(fileName_jsonDictionaryHotKeyCombinations);

                    string fileContent = file.ReadToEnd();

                    file.Close();

                    hotKeyCombinations = JsonSerializer.Deserialize<Dictionary<string, HotKeyCombination>>(fileContent);
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
            ServiceResponseStatus status = ServiceResponseStatus.OK;
            try
            {
                string jsonHotKeyCombinations = JsonSerializer.Serialize<Dictionary<string, HotKeyCombination>>(hotKeyCombinations);

                using FileStream fs = new FileStream(fileName_jsonDictionaryHotKeyCombinations, FileMode.Create);
                try
                {
                    using StreamWriter file = new StreamWriter(fs);
                    try
                    {
                        file.Write(jsonHotKeyCombinations);
                    }
                    catch { status = ServiceResponseStatus.FAILED; }
                    finally { file.Dispose(); }
                }
                catch { status = ServiceResponseStatus.FAILED; }
                finally { fs.Dispose(); }

            }
            catch { status = ServiceResponseStatus.FAILED; }
            return status;
        }


        public HotKeyCombination GetCombinationForName(string combinationName)
        {
            if (combinationName == null)
                return null;

            HotKeyCombination res;
            if (hotKeyCombinations.TryGetValue(combinationName, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public bool UpdateCombinationForName(string combinationName, HotKeyCombination hotKeyCombination)
        {
            /*
            if (hotKeyCombination.Keys.Count + hotKeyCombination.ModifierKeys.Count < 1)
            {
                return false;
            }
            */
            hotKeyCombinations[combinationName] = hotKeyCombination;
            return true;
        }
    }
}
