
namespace UncorRTDPS.Services
{
    interface Service
    {
        ServiceResponseStatus InitService(string[] args);
        ServiceResponseStatus CloseService();
    }
}
