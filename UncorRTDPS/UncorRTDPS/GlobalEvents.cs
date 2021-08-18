using System;

namespace UncorRTDPS
{
    public static class GlobalEvents
    {
        public static event EventHandler SettingsWindowClosed;

        public static void InvokeSettingsWindowClosed(object sender, EventArgs e)
        {
            SettingsWindowClosed?.Invoke(sender, e);
        }
    }
}
