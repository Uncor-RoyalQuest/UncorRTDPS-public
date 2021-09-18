using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UncorRTDPS.Services;
using UncorRTDPS.UncorRTDPS_Windows.EventAware;
using UncorRTDPS.UncorRTDPS_Windows.SettingsPanels;

namespace UncorRTDPS.UncorRTDPS_Windows
{
    /// <summary>
    /// Interaction logic for StatsSettingsHoveringWindow.xaml
    /// </summary>
    public partial class StatsSettingsHoveringWindow : Window, IParentTopmostListener, IDisposable, ICloseAllWindowsButMainAware
    {
        private const string name = "settingsWindow_000";

        public enum MenuOptions
        {
            None, CaptureArea, DPSAccuracy, Performance, Appearance, HotKeys, AboutProgram
        }

        private MenuOptions nowSelectedOption = MenuOptions.CaptureArea;
        private string selectedIndicator = ">";
        private string notSelectedIndicator = "";

        private List<IMenuPanel> menuPanels = new List<IMenuPanel>();
        private List<TextBlock> menuArrows = new List<TextBlock>();

        private bool oughtToBeTopmost = true;

        private WindowPositionService windowPositionService = null;

        public bool IsStartupPositionUnknown { get; set; } = true;

        public StatsSettingsHoveringWindow()
        {
            InitializeComponent();
            InitLocaleText();
            SubscribeTopmostChanger();

            menuPanels.Add(Panel_Settings_CaptureArea);
            menuArrows.Add(TextBlock_MenuArrow_CaptureArea);

            menuPanels.Add(Panel_Settings_DPSAccuracy);
            menuArrows.Add(TextBlock_MenuArrow_DPSAccuracy);

            menuPanels.Add(Panel_Settings_Performance);
            menuArrows.Add(TextBlock_MenuArrow_Performance);

            menuPanels.Add(Panel_Settings_Appearance);
            menuArrows.Add(TextBlock_MenuArrow_Appearance);

            menuPanels.Add(Panel_Settings_HotKeys);
            menuArrows.Add(TextBlock_MenuArrow_HotKeys);

            menuPanels.Add(Panel_Settings_AboutProgram);
            menuArrows.Add(TextBlock_MenuArrow_AboutProgram);

            Service service = ServicesContainer.GetService("windowPositionService");
            if (service != null && service is WindowPositionService)
            {
                windowPositionService = service as WindowPositionService;
                Point<double> p = windowPositionService.GetWindowPosition(name);
                if (p != null)
                {
                    this.Top = p.Y;
                    this.Left = p.X;
                    IsStartupPositionUnknown = false;
                }
            }

            (this as ICloseAllWindowsButMainAware).Register_CloseAllWindowsButMainAware();
        }

        public void InitLocaleText()
        {
            TextBlock_CaptureArea.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCaptureArea");
            TextBlock_DPSAccuracy.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiDPSAccuracy");
            TextBlock_Performance.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiPerformance");
            TextBlock_Appearance.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiAppearance");
            TextBlock_HotKeys.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiHotKeys");
            TextBlock_AboutProgram.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiAboutProgram");
        }

        public void SubscribeTopmostChanger()
        {
            Panel_Settings_CaptureArea.RegisterIParentTopmostListener(this);
        }

        public void FireTopmostChanged(bool topmost)
        {
            oughtToBeTopmost = topmost;
            this.Topmost = topmost;
            if (oughtToBeTopmost)
                this.Activate();
        }

        private bool isDisposed = false;
        public void Dispose()
        {
            if (isDisposed)
                return;
            isDisposed = true;
            Panel_Settings_CaptureArea.UnregisterIParentTopmostListener(this);

            TextBlock_CaptureArea.Text = null;
            TextBlock_DPSAccuracy.Text = null;
            TextBlock_Performance.Text = null;
            TextBlock_Appearance.Text = null;
            TextBlock_HotKeys.Text = null;
            TextBlock_AboutProgram.Text = null;

            Panel_Settings_CaptureArea.Dispose();
            Panel_Settings_DPSAccuracy.Dispose();
            Panel_Settings_Performance.Dispose();
            Panel_Settings_HotKeys.Dispose();
            Panel_Settings_Appearance.Dispose();

            menuPanels.Clear();
            menuArrows.Clear();

            windowPositionService = null;
        }

        private void Window_LostFocus(object sender, RoutedEventArgs e)
        {
            if (oughtToBeTopmost)
            {
                this.Topmost = false;
                this.Topmost = true;
            }
        }


        public void ActivateMenuOptionAndDeactivateOthers(int o)
        {
            for (int i = 0; i < menuPanels.Count; i++)
            {
                if (i == o)
                {
                    menuPanels[i].ActivateMenuPanel();
                    menuArrows[i].Text = selectedIndicator;
                }
                else
                {
                    menuPanels[i].DeactivateMenuPanel();
                    menuArrows[i].Text = notSelectedIndicator;
                }
            }

            if (menuPanels[o].GetMenuName() != "hotkeys" && nowSelectedOption == MenuOptions.HotKeys)
            {
                HotkeyMenuUnselected();
            }
        }

        private void Button_Menu_CaptureArea_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.CaptureArea)
            {
                ActivateMenuOptionAndDeactivateOthers(0);
                nowSelectedOption = MenuOptions.CaptureArea;
            }
        }

        private void Button_Menu_DPSAccuracy_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.DPSAccuracy)
            {
                ActivateMenuOptionAndDeactivateOthers(1);
                nowSelectedOption = MenuOptions.DPSAccuracy;
            }
        }

        private void Button_Menu_Performance_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.Performance)
            {
                ActivateMenuOptionAndDeactivateOthers(2);
                nowSelectedOption = MenuOptions.Performance;
            }
        }

        private void Button_Menu_Appearance_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.Appearance)
            {
                ActivateMenuOptionAndDeactivateOthers(3);
                nowSelectedOption = MenuOptions.Appearance;
            }
        }

        private void Button_Menu_AboutProgram_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.AboutProgram)
            {
                ActivateMenuOptionAndDeactivateOthers(5);
                nowSelectedOption = MenuOptions.AboutProgram;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (windowPositionService != null)
            {
                windowPositionService.UpdateWindowPosition(name, new Point<double>(this.Left, this.Top));
            }
            Dispose();
        }

        private void Window_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
        {

        }

        private void Button_Menu_HotKeys_Click(object sender, RoutedEventArgs e)
        {
            if (nowSelectedOption != MenuOptions.HotKeys)
            {
                ActivateMenuOptionAndDeactivateOthers(4);
                nowSelectedOption = MenuOptions.HotKeys;

                HotkeyMenuSelected();
            }
        }

        private void HotkeyMenuSelected()
        {
            GlobalEvents.InvokeHotkeysSettingsOpened(this, null);
        }

        private void HotkeyMenuUnselected()
        {
            GlobalEvents.InvokeHotkeysSettingsClosed(this, null);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (this as ICloseAllWindowsButMainAware).Unregister_CloseAllWindowsButMainAware();
            if (nowSelectedOption == MenuOptions.HotKeys)
            {
                GlobalEvents.InvokeHotkeysSettingsClosed(this, null);
            }
            GlobalEvents.InvokeSettingsWindowClosed(this, null);
        }

        private bool openedEventFired = false;
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!openedEventFired && this.Visibility == Visibility.Visible)
            {
                openedEventFired = true;
                GlobalEvents.InvokeSettingsWindowOpened(this, null);
            }
        }

        public void CloseAllWindowsButMainAware_HandleEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
