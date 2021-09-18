using System;

namespace UncorRTDPS.UncorRTDPS_Windows.EventAware
{
    public interface ICloseAllWindowsChartsAware
    {
        void Register_CloseAllWindowsChartsAware() => GlobalEvents.CloseAllWindowsCharts += CloseAllWindowsChartsAware_HandleEvent;

        void Unregister_CloseAllWindowsChartsAware() => GlobalEvents.CloseAllWindowsCharts -= CloseAllWindowsChartsAware_HandleEvent;

        void CloseAllWindowsChartsAware_HandleEvent(object sender, EventArgs e);
    }
}
