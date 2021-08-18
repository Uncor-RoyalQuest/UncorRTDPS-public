
namespace UncorRTDPS.Services
{
    interface Service
    {
        ServiceResponseStatus InitService(string param);
        ServiceResponseStatus CloseService();
    }
}
