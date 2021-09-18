using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UncorRTDPS.Services.WindowSize
{
    class WindowSizeService : Service
    {
        private Dictionary<string, Size<double>> windowsSizes = new Dictionary<string, Size<double>>();
        private string fileName_jsonDictionaryWindowsSizes = null;

        /// <summary>
        /// args[0] = fileName_jsonDictionaryWindowsSizes
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ServiceResponseStatus InitService(string[] args)
        {
            if (args == null || args.Length < 1)
                return ServiceResponseStatus.FAILED;
            string fileName_jsonDictionaryWindowsSizes = args[0];
            this.fileName_jsonDictionaryWindowsSizes = fileName_jsonDictionaryWindowsSizes;
            try
            {
                if (File.Exists(fileName_jsonDictionaryWindowsSizes))
                {
                    StreamReader file = new StreamReader(fileName_jsonDictionaryWindowsSizes);

                    string fileContent = file.ReadToEnd();

                    file.Close();

                    windowsSizes = JsonSerializer.Deserialize<Dictionary<string, Size<double>>>(fileContent);
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
                string jsonWindowsSizes = JsonSerializer.Serialize<Dictionary<string, Size<double>>>(windowsSizes);

                using (FileStream fs = new FileStream(fileName_jsonDictionaryWindowsSizes, FileMode.Create))
                {
                    try
                    {
                        using (StreamWriter file = new StreamWriter(fs))
                        {
                            file.Write(jsonWindowsSizes);
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


        public Size<double> GetWindowPosition(string windowUniqueId)
        {
            if (windowsSizes == null)
                return null;

            Size<double> res;
            if (windowsSizes.TryGetValue(windowUniqueId, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public void UpdateWindowSize(string windowUniqueId, Size<double> size)
        {
            windowsSizes[windowUniqueId] = size;
        }
    }
}
