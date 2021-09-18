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

        public static event EventHandler SettingsWindowOpened;

        public static void InvokeSettingsWindowOpened(object sender, EventArgs e)
        {
            SettingsWindowOpened?.Invoke(sender, e);
        }

        public static event EventHandler HotkeysSettingsOpened;

        public static void InvokeHotkeysSettingsOpened(object sender, EventArgs e)
        {
            HotkeysSettingsOpened?.Invoke(sender, e);
        }

        public static event EventHandler HotkeysSettingsClosed;

        public static void InvokeHotkeysSettingsClosed(object sender, EventArgs e)
        {
            HotkeysSettingsClosed?.Invoke(sender, e);
        }

        public static event EventHandler CloseAllWindowsButMain;

        public static void InvokeCloseAllWindowsButMain(object sender, EventArgs e)
        {
            CloseAllWindowsButMain?.Invoke(sender, e);
        }

        public static event EventHandler CloseAllWindowsCharts;

        public static void InvokeCloseAllWindowsCharts(object sender, EventArgs e)
        {
            CloseAllWindowsCharts?.Invoke(sender, e);
        }
    }
}
