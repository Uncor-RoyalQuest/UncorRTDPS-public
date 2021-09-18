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
        private HotKeysStorageService hotKeysStorageService = null;
        private string menu_name = "hotkeys";

        public S_HotKeys()
        {
            InitializeComponent();

            InitLocalization();

            hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;

            RefreshHotKeyView(CombinationNames.CombinationName_StartMonitoring, TextBlock_StartMonitoring_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_StopMonitoring, TextBlock_StopMonitoring_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleMonitoring, TextBlock_ToggleMonitoring_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_ResetAllStatistics, TextBlock_ResetStatistics_HotKey);

            RefreshHotKeyView(CombinationNames.CombinationName_OpenSettings, TextBlock_OpenSettings_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseSettings, TextBlock_CloseSettings_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleSettings, TextBlock_ToggleSettings_HotKey);

            RefreshHotKeyView(CombinationNames.CombinationName_OpenDamageHistory, TextBlock_OpenDamageHistory_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseDamageHistory, TextBlock_CloseDamageHistory_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleDamageHistory, TextBlock_ToggleDamageHistory_HotKey);

            RefreshHotKeyView(CombinationNames.CombinationName_OpenFirstNonCommonDetailedDamage, TextBlock_OpenLastDetailed_HotKey);

            RefreshHotKeyView(CombinationNames.CombinationName_CloseAllButMain, TextBlock_CloseAllExceptMain_HotKey);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseAllCharts, TextBlock_CloseAllCharts_HotKey);
        }

        public void InitLocalization()
        {
            TextBlock_ResetStatistics.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiResetAllStatistics");

            TextBlock_StartMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiStartMonitoring");
            TextBlock_StopMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiStopMonitoring");
            TextBlock_ToggleMonitoring.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiToggleMonitoring");

            TextBlock_OpenSettings.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiOpenSettings");
            TextBlock_CloseSettings.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCloseSettings");
            TextBlock_ToggleSettings.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiToggleSettings");

            TextBlock_OpenDamageHistory.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiOpenDamageHistory");
            TextBlock_CloseDamageHistory.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCloseDamageHistory");
            TextBlock_ToggleDamageHistory.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiToggleDamageHistory");

            TextBlock_OpenLastDetailed.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiOpenLastDetailed");

            TextBlock_CloseAllExceptMain.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCloseAllButMain");
            TextBlock_CloseAllCharts.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCloseAllDetailed");

            //buttons
            string guiBittonEditHotkeyText = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiButtonEditHotkey");
            Button_ResetStatistics_KeyPickerDialog.Content = guiBittonEditHotkeyText;

            Button_StartMonitoring_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_StopMonitoring_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_ToggleMonitoring_KeyPickerDialog.Content = guiBittonEditHotkeyText;

            Button_OpenSettings_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_CloseSettings_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_ToggleSettings_KeyPickerDialog.Content = guiBittonEditHotkeyText;

            Button_OpenDamageHistory_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_CloseDamageHistory_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_ToggleDamageHistory_KeyPickerDialog.Content = guiBittonEditHotkeyText;

            Button_OpenLastDetailed_KeyPickerDialog.Content = guiBittonEditHotkeyText;

            Button_CloseAllExceptMain_KeyPickerDialog.Content = guiBittonEditHotkeyText;
            Button_CloseAllCharts_KeyPickerDialog.Content = guiBittonEditHotkeyText;
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
            this.hotKeysStorageService = null;
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
            if (hotKeysStorageService == null)
                return;
            HotKeyCombination hotKeyCombination = hotKeysStorageService.GetCombinationForName(hotkeyName);
            if (hotKeyCombination != null)
            {
                keyPickerDialog.SetKeysByCopy(hotKeyCombination.ModifierKeys, hotKeyCombination.Keys);
            }

            //show dialog
            keyPickerDialog.ShowDialog();
            if (keyPickerDialog.IsToSave)
            {
                hotKeysStorageService.UpdateCombinationForName(hotkeyName, new HotKeyCombination(keyPickerDialog.GetResultModifiers(), keyPickerDialog.GetResultKeys()));
            }

            keyPickerDialog.Dispose();
        }

        public void RefreshHotKeyView(string hotkeyName, TextBlock textBlock)
        {
            if (hotKeysStorageService == null)
                return;
            HotKeyCombination hotKeyCombination = hotKeysStorageService.GetCombinationForName(hotkeyName);
            textBlock.Text = hotKeyCombination == null ? "" : hotKeyCombination.ToString();
        }

        private void Button_ResetStatistics_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_ResetAllStatistics);
            RefreshHotKeyView(CombinationNames.CombinationName_ResetAllStatistics, TextBlock_ResetStatistics_HotKey);
        }

        private void Button_StartMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_StartMonitoring);
            RefreshHotKeyView(CombinationNames.CombinationName_StartMonitoring, TextBlock_StartMonitoring_HotKey);
        }

        private void Button_StopMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_StopMonitoring);
            RefreshHotKeyView(CombinationNames.CombinationName_StopMonitoring, TextBlock_StopMonitoring_HotKey);
        }

        private void Button_ToggleMonitoring_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_ToggleMonitoring);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleMonitoring, TextBlock_ToggleMonitoring_HotKey);
        }

        private void Button_OpenSettings_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_OpenSettings);
            RefreshHotKeyView(CombinationNames.CombinationName_OpenSettings, TextBlock_OpenSettings_HotKey);
        }

        private void Button_CloseSettings_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_CloseSettings);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseSettings, TextBlock_CloseSettings_HotKey);
        }

        private void Button_ToggleSettings_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_ToggleSettings);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleSettings, TextBlock_ToggleSettings_HotKey);
        }

        private void Button_OpenDamageHistory_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_OpenDamageHistory);
            RefreshHotKeyView(CombinationNames.CombinationName_OpenDamageHistory, TextBlock_OpenDamageHistory_HotKey);
        }

        private void Button_CloseDamageHistory_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_CloseDamageHistory);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseDamageHistory, TextBlock_CloseDamageHistory_HotKey);
        }

        private void Button_ToggleDamageHistory_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_ToggleDamageHistory);
            RefreshHotKeyView(CombinationNames.CombinationName_ToggleDamageHistory, TextBlock_ToggleDamageHistory_HotKey);
        }

        public string GetMenuName()
        {
            return this.menu_name;
        }

        private void Button_CloseAllExceptMain_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_CloseAllButMain);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseAllButMain, TextBlock_CloseAllExceptMain_HotKey);
        }

        private void Button_CloseAllCharts_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_CloseAllCharts);
            RefreshHotKeyView(CombinationNames.CombinationName_CloseAllCharts, TextBlock_CloseAllCharts_HotKey);
        }

        private void Button_OpenLastDetailed_KeyPickerDialog_Click(object sender, RoutedEventArgs e)
        {
            ShowKeyPickerDialogForHotkeyName(CombinationNames.CombinationName_OpenFirstNonCommonDetailedDamage);
            RefreshHotKeyView(CombinationNames.CombinationName_OpenFirstNonCommonDetailedDamage, TextBlock_OpenLastDetailed_HotKey);
        }
    }
}
