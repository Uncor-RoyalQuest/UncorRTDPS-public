using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UncorRTDPS.Services;
using UncorRTDPS.Services.HotKeys;
using UncorRTDPS.Services.KeyPickerDialog;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_HotKeys.xaml
    /// </summary>
    public partial class S_HotKeys : UserControl, IMenuPanel, IDisposable
    {

        private const string combinationName_ResetAllStatistics = "resetAllStatistics";
        private const string combinationName_StartMonitoring = "startMonitoring";
        private const string combinationName_StopMonitoring = "stopMonitoring";
        private const string combinationName_ToggleMonitoring = "toggleMonitoring";

        private HotKeysStorageService hotKeysService = null;

        public S_HotKeys()
        {
            InitializeComponent();

            InitLocalization();

            hotKeysService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;

            RefreshHotKeyView(combinationName_StartMonitoring, TextBlock_StartMonitoring_HotKey);
            RefreshHotKeyView(combinationName_StopMonitoring, TextBlock_StopMonitoring_HotKey);
            RefreshHotKeyView(combinationName_ToggleMonitoring, TextBlock_ToggleMonitoring_HotKey);
            RefreshHotKeyView(combinationName_ResetAllStatistics, TextBlock_ResetStatistics_HotKey);
        }

        public void InitLocalization()
        {
            TextBlock_StartMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiStartMonitoring");
            TextBlock_StopMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiStopMonitoring");
            TextBlock_ToggleMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiToggleMonitoring");
            TextBlock_ResetStatistics.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiResetAllStatistics");

            Button_StartMonitoring_KeyPickerDialog.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonEditHotkey");
            Button_StopMonitoring_KeyPickerDialog.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonEditHotkey");
            Button_ToggleMonitoring_KeyPickerDialog.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonEditHotkey");
            Button_ResetStatistics_KeyPickerDialog.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonEditHotkey");
        }

        public void ActivateMenuPanel()
        {
            this.Visibility = Visibility.Visible;
        }

        public void DeactivateMenuPanel()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void Dispose()
        {
            this.hotKeysService = null;
        }

        private void ShowKeyPickerDialogForHotkeyName(string hotkeyName)
        {
            KeyPickerDialog keyPickerDialog = new KeyPickerDialog();

            //default position
            if (!keyPickerDialog.IsSavedWindowPositionSet)
            {
                Point pointToWindow = Mouse.GetPosition(this);
                Point pointToScreen = PointToScreen(pointToWindow);
                keyPickerDialog.Top = pointToScreen.Y;
                keyPickerDialog.Left = pointToScreen.X;
            }

            //set existing keys
            if (hotKeysService == null)
                return;
            HotKeyCombination hotKeyCombination = hotKeysService.GetCombinationForName(hotkeyName);
            if (hotKeyCombination != null)
            {
                keyPickerDialog.SetKeysByCopy(hotKeyCombination.ModifierKeys, hotKeyCombination.Keys);
            }

            //show dialog
            keyPickerDialog.ShowDialog();
            if (keyPickerDialog.IsToSave)
            {
                hotKeysService.UpdateCombinationForName(hotkeyName, new HotKeyCombination(keyPickerDialog.GetResultModifiers(), keyPickerDialog.GetResultKeys()));
            }

            keyPickerDialog.Dispose();
        }

        public void RefreshHotKeyView(string hotkeyName, TextBlock textBlock)
        {
            if (hotKeysService == null)
                return;
            HotKeyCombination hotKeyCombination = hotKeysService.GetCombinationForName(hotkeyName);
            textBlock.Text = hotKeyCombination == null ? "" : hotKeyCombination.ToString();
        }

        private void Button_ResetStatistics_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(combinationName_ResetAllStatistics);
            RefreshHotKeyView(combinationName_ResetAllStatistics, TextBlock_ResetStatistics_HotKey);
        }

        private void Button_StartMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(combinationName_StartMonitoring);
            RefreshHotKeyView(combinationName_StartMonitoring, TextBlock_StartMonitoring_HotKey);
        }

        private void Button_StopMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(combinationName_StopMonitoring);
            RefreshHotKeyView(combinationName_StopMonitoring, TextBlock_StopMonitoring_HotKey);
        }

        private void Button_ToggleMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(combinationName_ToggleMonitoring);
            RefreshHotKeyView(combinationName_ToggleMonitoring, TextBlock_ToggleMonitoring_HotKey);
        }
    }
}
