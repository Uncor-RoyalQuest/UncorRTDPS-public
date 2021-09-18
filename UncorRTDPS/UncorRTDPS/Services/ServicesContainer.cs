using System.Collections.Generic;

namespace UncorRTDPS.Services
{
    static class ServicesContainer
    {
        private static Dictionary<string, Service> services = new Dictionary<string, Service>();

        public static bool AddNewService(string serviceUniqueName, Service service)
        {
            if (services.ContainsKey(serviceUniqueName))
                return false;
            services.Add(serviceUniqueName, service);
            return true;
        }

        public static Service GetService(string serviceUniqueName)
        {
            if (services.ContainsKey(serviceUniqueName))
                return services[serviceUniqueName];
            else
                return null;
        }


        public static void CloseServicesContainer()
        {
            foreach (KeyValuePair<string, Service> kv in services)
            {
                kv.Value.CloseService();
            }
        }
    }
}
