using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace UncorRTDPS.Services
{
    class WindowPositionService : Service
    {
        private Dictionary<string, Point<double>> windowsPositions = new Dictionary<string, Point<double>>();
        private string fileName_jsonDictionaryWindowsPoints = null;

        public ServiceResponseStatus InitService(string fileName_jsonDictionaryWindowsPoints)
        {
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
            } catch
            {
                return ServiceResponseStatus.FAILED;
            }
            return ServiceResponseStatus.OK;
        }

        public ServiceResponseStatus CloseService()
        {
            try
            {
                string jsonWindowsPositions = JsonSerializer.Serialize<Dictionary<string, Point<double>>>(windowsPositions);

                using (FileStream fs = new FileStream(fileName_jsonDictionaryWindowsPoints, FileMode.Create))
                {
                    try
                    {
                        using (StreamWriter file = new StreamWriter(fs))
                        {
                            file.Write(jsonWindowsPositions);
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

        public static WindowPositionService createWindowPositionService()
        {
            return new WindowPositionService();
        }
    }
}
