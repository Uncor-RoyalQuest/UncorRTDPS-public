using System;

namespace UncorRTDPS.UncorRTDPS_Windows.EventAware
{
    public interface ICloseAllWindowsButMainAware
    {
        void Register_CloseAllWindowsButMainAware() => GlobalEvents.CloseAllWindowsButMain += CloseAllWindowsButMainAware_HandleEvent;

        void Unregister_CloseAllWindowsButMainAware() => GlobalEvents.CloseAllWindowsButMain -= CloseAllWindowsButMainAware_HandleEvent;

        void CloseAllWindowsButMainAware_HandleEvent(object sender, EventArgs e);
    }
}
