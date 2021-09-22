using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UncorRTDPS.Services
{
    class WindowPositionService : Service
    {
        private Dictionary<string, Point<double>> windowsPositions = new Dictionary<string, Point<double>>();
        private string fileName_jsonDictionaryWindowsPoints = null;

        /// <summary>
        /// args[0] = fileName_jsonDictionaryWindowsPoints
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public ServiceResponseStatus InitService(string[] args)
        {
            if (args == null || args.Length < 1)
                return ServiceResponseStatus.FAILED;
            string fileName_jsonDictionaryWindowsPoints = args[0];
            this.fileName_jsonDictionaryWindowsPoints = fileName_jsonDictionaryWindowsPoints;
            try
            {
                if (File.Exists(fileName_jsonDictionaryWindowsPoints))
                {
                    StreamReader file = new StreamReader(fileName_jsonDictionaryWindowsPoints);

                    string fileContent = file.ReadToEnd();

                    file.Close();

                    windowsPositions = JsonSerializer.Deserialize<Dictionary<string, Point<double>>>(fileContent);
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
                string jsonWindowsPositions = JsonSerializer.Serialize<Dictionary<string, Point<double>>>(windowsPositions);

                using FileStream fs = new FileStream(fileName_jsonDictionaryWindowsPoints, FileMode.Create);
                try
                {
                    using StreamWriter file = new StreamWriter(fs);
                    try { file.Write(jsonWindowsPositions); }
                    catch { status = ServiceResponseStatus.FAILED; }
                    finally { file.Dispose(); }
                }
                catch { status = ServiceResponseStatus.FAILED; }
                finally { fs.Dispose(); }

            }
            catch { status = ServiceResponseStatus.FAILED; }
            return status;
        }


        public Point<double> GetWindowPosition(string windowUniqueId)
        {
            if (windowsPositions == null)
                return null;

            Point<double> res;
            if (windowsPositions.TryGetValue(windowUniqueId, out res))
            {
                return res;
            }
            else
            {
                return null;
            }
        }

        public void UpdateWindowPosition(string windowUniqueId, Point<double> pos)
        {
            windowsPositions[windowUniqueId] = pos;
        }
    }
}
