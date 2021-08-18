using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UncorRTDPS.Services.HotKeys
{
    class HotKeysStorageService : Service
    {

        private Dictionary<string, HotKeyCombination> hotKeyCombinations = new Dictionary<string, HotKeyCombination>();
        private string fileName_jsonDictionaryHotKeyCombinations;

        public ServiceResponseStatus InitService(string fileName_jsonDictionaryHotKeyCombinations)
        {
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
            try
            {
                string jsonHotKeyCombinations = JsonSerializer.Serialize<Dictionary<string, HotKeyCombination>>(hotKeyCombinations);

                using (FileStream fs = new FileStream(fileName_jsonDictionaryHotKeyCombinations, FileMode.Create))
                {
                    try
                    {
                        using (StreamWriter file = new StreamWriter(fs))
                        {
                            file.Write(jsonHotKeyCombinations);
                        }
                    }
                    catch
                    {
                        return ServiceResponseStatus.FAILED;
                    }
                    finally
                    {
                        fs.Dispose();
                    }
                }

            }
            catch
            {
                return ServiceResponseStatus.FAILED;
            }
            return ServiceResponseStatus.OK;
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

        public static HotKeysStorageService createHotKeysStorageService()
        {
            return new HotKeysStorageService();
        }
    }
}
