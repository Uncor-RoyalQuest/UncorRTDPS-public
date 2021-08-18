using System;
using System.Windows;
using System.Windows.Controls;

namespace UncorRTDPS.UncorRTDPS_Windows.SettingsPanels
{
    /// <summary>
    /// Interaction logic for S_Performance.xaml
    /// </summary>
    public partial class S_Performance : UserControl, IMenuPanel, IDisposable
    {
        public S_Performance()
        {
            InitializeComponent();
            InitLocaleText();
            RefreshMenuValues();
        }

        public void InitLocaleText()
        {
            TextBlock_DmgSepDel_Boss.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiDmgSepDelay_Boss");
            TextBlock_DmgSepDel_Elite.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiDmgSepDelay_Elite");
            TextBlock_DmgSepDel_Common.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiDmgSepDelay_Common");

            TextBlock_UpdateCountLimiter.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit");
            string s_tooltip_active =
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_Active_tooltip_1")
                +
                Environment.NewLine
                +
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_1")
                + 
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_2")
                + 
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_3");
            TextBlock_UpdateCountLimiter.ToolTip = s_tooltip_active;

            TextBlock_UpdateCountLimiterInactive.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_Inactive");
            string s_tooltip_inactive =
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_Inactive_tooltip_1")
                +
                Environment.NewLine
                +
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_1")
                +
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_2")
                +
                Environment.NewLine
                +
                RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiUpdPerSecLimit_tooltip_3");
            TextBlock_UpdateCountLimiterInactive.ToolTip = s_tooltip_inactive;

            TextBlock_Comment_ApplyNewSettings.Text = "";
            Button_ApplyNewSettings.Content = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiApplyNewPerformance");
        }

        public void SetApplyComment_Applied()
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

        private void Button_ApplyNewSettings_Click(object sender, RoutedEventArgs e)
        {
            long dmgSepBoss = (long)(Slider_DmgSepDel_Boss_Value.Value * 1000);
            long dmgSepElite = (long)(Slider_DmgSepDel_Elite_Value.Value * 1000);
            long dmgSepCommon = (long)(Slider_DmgSepDel_Common_Value.Value * 1000);

            double rpsLimitActive = Slider_UpdateCountLimiter_Value.Value;
            double rpsLimitInactive = Slider_UpdateCountLimiterInactive_Value.Value;
            
            
            if (double.IsNaN(dmgSepBoss) ||
                double.IsNaN(dmgSepElite) ||
                double.IsNaN(dmgSepCommon) ||
                double.IsNaN(rpsLimitActive) ||
                double.IsNaN(rpsLimitInactive))
            {
                SetApplyComment_Error();
                return;
            }


            //UncorRTDPS_Settings.UncorRTDPS_StaticSettings.updateDPSModelSettings(new UncorDpsModels.ModelSettings((long)(damageSeparationDelay * 1000)));
            RTDPS_Settings.UncorRTDPS_StaticSettings.UpdatePerformanceSettings(
                new DpsModels.ModelSettings(dmgSepBoss, dmgSepElite, dmgSepCommon),
                rpsLimitActive,
                rpsLimitInactive);
            SetApplyComment_Applied();
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
            Slider_DmgSepDel_Boss_Value.Value = (double)RTDPS_Settings.UncorRTDPS_StaticSettings.DpsModelSettings.BossDamageSeparationDelayMS / 1000;
            Slider_DmgSepDel_Elite_Value.Value = (double)RTDPS_Settings.UncorRTDPS_StaticSettings.DpsModelSettings.EliteDamageSeparationDelayMS / 1000;
            Slider_DmgSepDel_Common_Value.Value = (double)RTDPS_Settings.UncorRTDPS_StaticSettings.DpsModelSettings.CommonDamageSeparationDelayMS / 1000;

            Slider_UpdateCountLimiter_Value.Value = RTDPS_Settings.UncorRTDPS_StaticSettings.RpsMonitoringLimit_Active;
            Slider_UpdateCountLimiterInactive_Value.Value = RTDPS_Settings.UncorRTDPS_StaticSettings.RpsMonitoringLimit_Inactive;
        }

        public void Dispose()
        {
            TextBlock_DmgSepDel_Boss.Text = null;
            TextBlock_DmgSepDel_Elite.Text = null;
            TextBlock_DmgSepDel_Common.Text = null;

            TextBlock_UpdateCountLimiter.Text = null;
            TextBlock_UpdateCountLimiter.ToolTip = null;

            TextBlock_Comment_ApplyNewSettings.Text = null;
            Button_ApplyNewSettings.Content = null;
        }
    }
}
