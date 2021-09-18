using System;
using System.Windows;
using System.Windows.Controls;
using UncorRTDPS.RTDPS_Settings;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_Appearance.xaml
    /// </summary>
    public partial class S_Appearance : UserControl, IMenuPanel, IDisposable
    {
        private string menu_name = "appearance";
        public S_Appearance()
        {
            InitializeComponent();
            InitLocaleText();
            RefreshMenuValues();
        }

        public void InitLocaleText()
        {
            TextBlock_VisualRefreshDelay.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiVisRefrDel");
            TextBlock_DPSFontSize.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiFontSize");
            TextBlock_StatWindowOpacity.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiStatWindowOpacity");

            TextBlock_ViewMode.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiDamageViewMode");
            TextBlock_ViewMode.ToolTip =
                UncorRTDPS_Localization.GetLocaleGuiVal("guiDamageViewMode_tooltip_1")
                +
                Environment.NewLine
                +
                UncorRTDPS_Localization.GetLocaleGuiVal("guiDamageViewMode_tooltip_2");
            ComboBox_Item_ViewMode_HighestPriority.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiDamageViewMode_Mode_0");
            ComboBox_Item_ViewMode_All.Content = UncorRTDPS_Localization.GetLocaleGuiVal("guiDamageViewMode_Mode_1");

            TextBlock_EnableAliases.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiEnableAliases");

            TextBlock_ShowInTheMode_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowInTheMode_Mode_0");
            TextBlock_ShowDmg_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowDmg_Mode_0");
            TextBlock_ShowHits_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowHits_Mode_0");
            TextBlock_ShowMaxHitDmg_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowMaxHitDmg_Mode_0");
            TextBlock_ShowMaxHitDmg_Mode_0.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowMaxHitDmg_Mode_0_Tooltip");
            TextBlock_ShowT_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowT_Mode_0");
            TextBlock_ShowDps_Mode_0.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowDps_Mode_0");

            TextBlock_ShowInTheMode_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowInTheMode_Mode_1");
            TextBlock_ShowDmg_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowDmg_Mode_1");
            TextBlock_ShowHits_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowHits_Mode_1");
            TextBlock_ShowMaxHitDmg_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowMaxHitDmg_Mode_1");
            TextBlock_ShowMaxHitDmg_Mode_1.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowMaxHitDmg_Mode_1_Tooltip");
            TextBlock_ShowT_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowT_Mode_1");
            TextBlock_ShowDps_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowDps_Mode_1");
            TextBlock_BossesLimit_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiMaxBosses_Mode_1");
            TextBlock_ElitesLimit_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiMaxElites_Mode_1");
            TextBlock_ShowCommon_Mode_1.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowCommon_Mode_1");

            TextBlock_ShowOcrStat.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats");
            TextBlock_ShowOcrStat_Failures.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_Failures");
            TextBlock_ShowOcrStat_Losses.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_Losses");
            TextBlock_ShowOcrStat_RPS.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_RPS");
            TextBlock_ShowOcrStat_ART.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_ART");
            TextBlock_ShowOcrStat_ALoad.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_ALoad");
            TextBlock_ShowOcrStat_MLoad.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiShowRecStats_MLoad");


            Button_ApplyNewSettings.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiApplyNewAppearance");
            TextBlock_Comment_ApplyNewSettings.Text = "";
        }

        private void Button_ApplyNewSettings_Click(object sender, RoutedEventArgs e)
        {
            int fontSize = (int)Slider_DPSFontSize_Value.Value;
            float visualRefreshDelay = (float)Slider_VisualRefreshDelay_Value.Value;
            float opacityDpsWindow = (float)Slider_StatWindowOpacity_Value.Value;
            int selectedViewDpsMode = ComboBox_ViewMode.SelectedIndex;
            //bool showOcrSimpleStat = CheckBox_ShowOcrSimpleStat.IsChecked == true ? true : false;
            //bool showOcrAdvancedStat = CheckBox_ShowOcrStat.IsChecked == true ? true : false;

            if (float.IsNaN(visualRefreshDelay) || float.IsNaN(opacityDpsWindow))
            {
                SetApplyComment_Error();
                return;
            }

            DPSWindowSettings dpsWindowSettings = new DPSWindowSettings(RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings);
            dpsWindowSettings.FontSize = fontSize;
            dpsWindowSettings.Opacity = opacityDpsWindow;
            dpsWindowSettings.VisualRefreshDelay = (long)(visualRefreshDelay * 1000);
            dpsWindowSettings.DpsViewMode = selectedViewDpsMode;

            dpsWindowSettings.EnableAliases = CheckBox_EnableAliases.IsChecked == true ? true : false;

            dpsWindowSettings.ShowDamage_mode_0 = CheckBox_ShowDmg_Mode_0.IsChecked == true ? true : false;
            dpsWindowSettings.ShowHits_mode_0 = CheckBox_ShowHits_Mode_0.IsChecked == true ? true : false;
            dpsWindowSettings.ShowMaxHitDmg_mode_0 = CheckBox_ShowMaxHitDmg_Mode_0.IsChecked == true ? true : false;
            dpsWindowSettings.ShowT_mode_0 = CheckBox_ShowT_Mode_0.IsChecked == true ? true : false;
            dpsWindowSettings.ShowDps_mode_0 = CheckBox_ShowDps_Mode_0.IsChecked == true ? true : false;

            dpsWindowSettings.ShowDamage_mode_1 = CheckBox_ShowDmg_Mode_1.IsChecked == true ? true : false;
            dpsWindowSettings.ShowHits_mode_1 = CheckBox_ShowHits_Mode_1.IsChecked == true ? true : false;
            dpsWindowSettings.ShowMaxHitDmg_mode_1 = CheckBox_ShowMaxHitDmg_Mode_1.IsChecked == true ? true : false;
            dpsWindowSettings.ShowT_mode_1 = CheckBox_ShowT_Mode_1.IsChecked == true ? true : false;
            dpsWindowSettings.ShowDps_mode_1 = CheckBox_ShowDps_Mode_1.IsChecked == true ? true : false;
            dpsWindowSettings.BossesLimit_mode_1 = (int)Slider_BossesLimit_Mode_1.Value;
            dpsWindowSettings.ElitesLimit_mode_1 = (int)Slider_ElitesLimit_Mode_1.Value;
            dpsWindowSettings.ShowCommonMobsDps_mode_1 = CheckBox_ShowCommon_Mode_1.IsChecked == true ? true : false;

            dpsWindowSettings.ShowOcrStat = CheckBox_ShowOcrStat.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_Failures = CheckBox_ShowOcrStat_Failures.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_Losses = CheckBox_ShowOcrStat_Losses.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_RPS = CheckBox_ShowOcrStat_RPS.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_ART = CheckBox_ShowOcrStat_ART.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_ALoad = CheckBox_ShowOcrStat_ALoad.IsChecked == true ? true : false;
            dpsWindowSettings.ShowOcrStat_MLoad = CheckBox_ShowOcrStat_MLoad.IsChecked == true ? true : false;


            RTDPS_Settings.UncorRTDPS_StaticSettings.UpdateDPSWindowSettings(dpsWindowSettings);
            setApplyComment_Applied();
        }

        public void setApplyComment_Applied()
        {
            TextBlock_Comment_ApplyNewSettings.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentApplied");
        }

        public void SetApplyComment_Error()
        {
            TextBlock_Comment_ApplyNewSettings.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiCommentError");
        }

        public void SetApplyComment_Clear()
        {
            if (TextBlock_Comment_ApplyNewSettings.Text == null || TextBlock_Comment_ApplyNewSettings.Text.Length == 0)
                return;
            TextBlock_Comment_ApplyNewSettings.Text = "";
        }

        public void ActivateMenuPanel()
        {
            this.Visibility = Visibility.Visible;
            RefreshMenuValues();
        }

        public void DeactivateMenuPanel()
        {
            this.Visibility = Visibility.Hidden;
        }

        public void RefreshMenuValues()
        {
            Slider_DPSFontSize_Value.Value = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.FontSize;
            Slider_StatWindowOpacity_Value.Value = RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.Opacity;
            Slider_VisualRefreshDelay_Value.Value = (double)RTDPS_Settings.UncorRTDPS_StaticSettings.DpsWindowSettings.VisualRefreshDelay / 1000;

            ComboBox_ViewMode.SelectedIndex = UncorRTDPS_StaticSettings.DpsWindowSettings.DpsViewMode;

            CheckBox_EnableAliases.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.EnableAliases;

            CheckBox_ShowDmg_Mode_0.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDamage_mode_0;
            CheckBox_ShowHits_Mode_0.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowHits_mode_0;
            CheckBox_ShowMaxHitDmg_Mode_0.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowMaxHitDmg_mode_0;
            CheckBox_ShowT_Mode_0.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowT_mode_0;
            CheckBox_ShowDps_Mode_0.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDps_mode_0;

            CheckBox_ShowDmg_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDamage_mode_1;
            CheckBox_ShowHits_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowHits_mode_1;
            CheckBox_ShowMaxHitDmg_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowMaxHitDmg_mode_1;
            CheckBox_ShowT_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowT_mode_1;
            CheckBox_ShowDps_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowDps_mode_1;
            Slider_BossesLimit_Mode_1.Value = UncorRTDPS_StaticSettings.DpsWindowSettings.BossesLimit_mode_1;
            Slider_ElitesLimit_Mode_1.Value = UncorRTDPS_StaticSettings.DpsWindowSettings.ElitesLimit_mode_1;
            CheckBox_ShowCommon_Mode_1.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowCommonMobsDps_mode_1;


            CheckBox_ShowOcrStat.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat;

            CheckBox_ShowOcrStat_Failures.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Failures;
            CheckBox_ShowOcrStat_Losses.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Losses;
            CheckBox_ShowOcrStat_RPS.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_RPS;
            CheckBox_ShowOcrStat_ART.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ART;
            CheckBox_ShowOcrStat_ALoad.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ALoad;
            CheckBox_ShowOcrStat_MLoad.IsChecked = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_MLoad;
        }

        public void Dispose()
        {
            TextBlock_VisualRefreshDelay.Text = null;
            TextBlock_DPSFontSize.Text = null;
            TextBlock_StatWindowOpacity.Text = null;
            TextBlock_Comment_ApplyNewSettings.Text = null;

            TextBlock_ViewMode.Text = null;
            TextBlock_ViewMode.ToolTip = null;
            ComboBox_Item_ViewMode_HighestPriority.Content = null;
            ComboBox_Item_ViewMode_All.Content = null;

            TextBlock_EnableAliases.Text = null;

            TextBlock_ShowInTheMode_Mode_0.Text = null;
            TextBlock_ShowDmg_Mode_0.Text = null;
            TextBlock_ShowHits_Mode_0.Text = null;
            TextBlock_ShowMaxHitDmg_Mode_0.Text = null;
            TextBlock_ShowMaxHitDmg_Mode_0.ToolTip = null;
            TextBlock_ShowT_Mode_0.Text = null;
            TextBlock_ShowDps_Mode_0.Text = null;

            TextBlock_ShowInTheMode_Mode_1.Text = null;
            TextBlock_ShowDmg_Mode_1.Text = null;
            TextBlock_ShowHits_Mode_1.Text = null;
            TextBlock_ShowMaxHitDmg_Mode_1.Text = null;
            TextBlock_ShowMaxHitDmg_Mode_1.ToolTip = null;
            TextBlock_ShowT_Mode_1.Text = null;
            TextBlock_ShowDps_Mode_1.Text = null;
            TextBlock_BossesLimit_Mode_1.Text = null;
            TextBlock_ElitesLimit_Mode_1.Text = null;
            TextBlock_ShowCommon_Mode_1.Text = null;

            TextBlock_ShowOcrStat.Text = null;
            TextBlock_ShowOcrStat_Failures.Text = null;
            TextBlock_ShowOcrStat_Losses.Text = null;
            TextBlock_ShowOcrStat_RPS.Text = null;
            TextBlock_ShowOcrStat_ART.Text = null;
            TextBlock_ShowOcrStat_ALoad.Text = null;
            TextBlock_ShowOcrStat_MLoad.Text = null;

            Button_ApplyNewSettings.Content = null;
        }

        public string GetMenuName()
        {
            return this.menu_name;
        }
    }
}
