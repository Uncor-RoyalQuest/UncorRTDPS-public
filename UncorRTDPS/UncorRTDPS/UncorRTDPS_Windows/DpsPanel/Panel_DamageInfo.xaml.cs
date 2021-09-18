using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using UncorRTDPS.DamageCharts;
using UncorRTDPS.DpsModels;
using UncorRTDPS.DpsModels.TargetsDictionary;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.Services;
using UncorRTDPS.Services.Aliasing;
using UncorRTDPS.Services.GlobalKeyPressListener;
using UncorRTDPS.Services.HotKeys;
using UncorRTDPS.Util;

namespace UncorRTDPS.UncorRTDPS_Windows.DpsPanel
{
    /// <summary>
    /// Interaction logic for Panel_DamageInfo.xaml
    /// </summary>
    public partial class Panel_DamageInfo : UserControl, IDisposable
    {
        public Panel_DamageInfo()
        {
            InitializeComponent();
            InitLocaleText();
            InitTooltips();
            windowDispatcher = Application.Current.Dispatcher;
            commonTargetName = UncorRTDPS_Localization.GetLocaleGuiVal("guiCommonMob");

            InitNumberFormat();

            RegisterHotkeys();

            this.simpleAliasService_Mobs = ServicesContainer.GetService("simpleAliasService_Mobs") as SimpleAliasService;

            GlobalEvents.SettingsWindowClosed += SettingsWindowClosed_HandleEvent;

            //Color bgColor = (Color)ColorConverter.ConvertFromString("#FF2D2D30");
            //TextBlock_TargetName_DropShadowEffect.Color = bgColor;
        }

        private Brush defaultForeground = new SolidColorBrush(Color.FromArgb(255, 171, 171, 167));
        private Brush selectedForeground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 55));
        private Dispatcher windowDispatcher;
        private string commonTargetName;
        private DamageModel lastDamageModel = null;
        private Target lastTarget = null;
        private NumberFormatInfo numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = " " };
        private string uid = "panel_dmgInfo_" + UniqueLongGenerator.GetUniqueId();

        private string currentTarget_OriginalName = "";

        private SimpleAliasService simpleAliasService_Mobs = null;

        private bool isHotKeysRegistered = false;
        public void RegisterHotkeys()
        {
            if (isHotKeysRegistered)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotKeysStorageService hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;
            if (hotKeysStorageService == null)
                return;


            //create hotkey
            HotKeyCombination hotKeyCombination_ResetAllStatistics = hotKeysStorageService.GetCombinationForName("resetAllStatistics");
            if (hotKeyCombination_ResetAllStatistics == null)
                return;

            KeyPressSequence keyPressSequence = new KeyPressSequence(hotKeyCombination_ResetAllStatistics);
            keyPressSequence.DelegatedAction = () =>
            {
                ResetStatistics();
            };


            //register hotkey resetAllStatistics
            if (keyPressSequence.SequenceLength > 0)
            {
                globalKeyPressService.RegisterEventHandler_OnKeyPressed(uid + "resetAllStatistics", keyPressSequence);
            }

            isHotKeysRegistered = true;
        }

        public void UnregisterHotkeys()
        {
            if (!isHotKeysRegistered)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            globalKeyPressService.UnregisterEventHandler_OnKeyPressed(uid + "resetAllStatistics");

            isHotKeysRegistered = false;
        }

        public void InitNumberFormat()
        {
            string nf = UncorRTDPS_Config.getConfigVal("numberFormat_ThrousandsSeparator");
            if (nf == null || nf.Length < 1 || nf.Equals("0"))
                nf = " ";
            numberFormatInfo_FancyLong = new NumberFormatInfo { NumberGroupSeparator = nf };
        }

        public void InitLocaleText()
        {
            ContextMenu_Item_CopyDmg.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyDmg");
            ContextMenu_Item_CopyT.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyT");
            ContextMenu_Item_CopyDps.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyDps");
            ContextMenu_Item_CopyHits.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyHits");
            ContextMenu_Item_CopyMaxHitDmg.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyMaxHitDmg");
            ContextMenu_Item_CopyAll.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiCopyAll");

            ContextMenu_Item_ResetStat.Header = UncorRTDPS_Localization.GetLocaleGuiVal("guiResetStatistics");

        }

        public void InitTooltips()
        {
            TextBlock_DMG.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiDMG_tooltip");
            TextBlock_Hits.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiHits_tooltip");
            TextBlock_MaxHitDmg.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiMaxHitDmg_tooltip");
            TextBlock_T.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiT_tooltip");
            TextBlock_DPS.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiDPS_tooltip");
        }

        public void UpdateViewData(DamageModel damageModel, Target target)
        {

            bool isResetAsked = damageModel.IsResetAsked();
            if (isResetAsked && lastTarget != target)
            {
                UpdTargetNameInfo(target, damageModel.TotalDamage);
                lastTarget = target;
                return;
            }
            if (isResetAsked)
                return;

            //dmg
            TextBlock_DMG_Value.Text = damageModel.TotalDamage.ToString("#,0", numberFormatInfo_FancyLong);

            //hits
            TextBlock_Hits_Value.Text = damageModel.Hits.ToString("#,0", numberFormatInfo_FancyLong);

            //max dmg
            TextBlock_MaxHitDmg_Value.Text = damageModel.MaxHitDmg.ToString("#,0", numberFormatInfo_FancyLong);

            //t
            long dmgTime = damageModel.CalcDamageTime() / 1000;
            TextBlock_T_Value.Text = dmgTime.ToString("#,0", numberFormatInfo_FancyLong);

            //dps
            if (dmgTime == 0)
                dmgTime = 1;
            TextBlock_DPS_Value.Text = damageModel.CalcDps().ToString("#,0", numberFormatInfo_FancyLong);

            //target name
            UpdTargetNameInfo(target, damageModel.TotalDamage);

            lastDamageModel = damageModel;
            lastTarget = target;
        }

        private void UpdTargetNameInfo(Target target, long damageDealt)
        {
            if (target == null)
            {
                if (currentTarget_OriginalName != commonTargetName)
                {
                    currentTarget_OriginalName = commonTargetName;
                    TextBlock_TargetName.Text = commonTargetName;
                    TextBlock_TargetName.ToolTip = commonTargetName;
                    TextBlock_DMG_PercentValue.Text = "";
                }
            }
            else
            {
                if (currentTarget_OriginalName != target.originalName)
                {
                    currentTarget_OriginalName = target.originalName;
                    TextBlock_TargetName.ToolTip = target.originalName;

                    //is aliasing enabled
                    //if true then try get alias for target and apply that alias
                    if (UncorRTDPS_StaticSettings.DpsWindowSettings.EnableAliases && simpleAliasService_Mobs != null)
                    {
                        string alias = simpleAliasService_Mobs.GetAliasForName(target.originalName);
                        if (alias != null)
                            TextBlock_TargetName.Text = alias;
                        else
                            TextBlock_TargetName.Text = target.originalName;
                    }
                    else TextBlock_TargetName.Text = target.originalName;

                }

                //dmg percent value
                string s_perc = GetPercentFromDamageAndHp(damageDealt, target.hp);
                if (s_perc != null)
                {
                    TextBlock_DMG_PercentValue.Text = s_perc;
                }
                else if (TextBlock_DMG_PercentValue.Text.Length > 0)
                {
                    TextBlock_DMG_PercentValue.Text = "";
                }
            }
        }

        public string GetPercentFromDamageAndHp(long dmg, long hp)
        {
            if (hp < 1)
                return null;
            return "(" + String.Format("{0:0.##}", ((double)dmg / hp) * 100) + "%)";
        }

        private long lastLeftClick_TextBlock_DMG_Value = 0;
        private void TextBlock_DMG_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeNow - lastLeftClick_TextBlock_DMG_Value < 2000)
            {
                //double click confirmed
                TextBlock_DMG.Foreground = selectedForeground;
                TextBlock_DMG_Value.Foreground = selectedForeground;

                Task.Delay(100).ContinueWith(_ =>
                {
                    windowDispatcher.Invoke(SetTextBlocksColor_DMG_ToDefault, System.Windows.Threading.DispatcherPriority.Normal);
                }
                );
                Clipboard.SetText(TextBlock_DMG_Value.Text);
                lastLeftClick_TextBlock_DMG_Value = 0;
            }
            else
            {
                lastLeftClick_TextBlock_DMG_Value = timeNow;
            }
        }

        private long lastLeftClick_TextBlock_Hits_Value = 0;
        private void TextBlock_Hits_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeNow - lastLeftClick_TextBlock_Hits_Value < 500)
            {
                //double click confirmed
                TextBlock_Hits.Foreground = selectedForeground;
                TextBlock_Hits_Value.Foreground = selectedForeground;

                Task.Delay(100).ContinueWith(_ =>
                {
                    windowDispatcher.Invoke(SetTextBlocksColor_Hits_ToDefault, System.Windows.Threading.DispatcherPriority.Normal);
                }
                );

                Clipboard.SetText(TextBlock_Hits_Value.Text);
                lastLeftClick_TextBlock_Hits_Value = 0;
            }
            else
            {
                lastLeftClick_TextBlock_Hits_Value = timeNow;
            }
        }

        private long lastLeftClick_TextBlock_MaxHitDmg_Value = 0;
        private void TextBlock_MaxHitDmg_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeNow - lastLeftClick_TextBlock_MaxHitDmg_Value < 500)
            {
                //double click confirmed
                TextBlock_MaxHitDmg.Foreground = selectedForeground;
                TextBlock_MaxHitDmg_Value.Foreground = selectedForeground;

                Task.Delay(100).ContinueWith(_ =>
                {
                    windowDispatcher.Invoke(SetTextBlocksColor_MaxHitDmg_ToDefault, System.Windows.Threading.DispatcherPriority.Normal);
                }
                );

                Clipboard.SetText(TextBlock_MaxHitDmg_Value.Text);
                lastLeftClick_TextBlock_MaxHitDmg_Value = 0;
            }
            else
            {
                lastLeftClick_TextBlock_MaxHitDmg_Value = timeNow;
            }
        }

        private long lastLeftClick_TextBlock_T_Value = 0;
        private void TextBlock_T_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeNow - lastLeftClick_TextBlock_T_Value < 500)
            {
                //double click confirmed
                TextBlock_T.Foreground = selectedForeground;
                TextBlock_T_Value.Foreground = selectedForeground;

                Task.Delay(100).ContinueWith(_ =>
                {
                    windowDispatcher.Invoke(SetTextBlocksColor_T_ToDefault, System.Windows.Threading.DispatcherPriority.Normal);
                }
                );

                Clipboard.SetText(TextBlock_T_Value.Text);
                lastLeftClick_TextBlock_T_Value = 0;
            }
            else
            {
                lastLeftClick_TextBlock_T_Value = timeNow;
            }
        }

        private long lastLeftClick_TextBlock_DPS_Value = 0;
        private void TextBlock_DPS_Value_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            long timeNow = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            if (timeNow - lastLeftClick_TextBlock_DPS_Value < 2000)
            {
                //double click confirmed
                TextBlock_DPS.Foreground = selectedForeground;
                TextBlock_DPS_Value.Foreground = selectedForeground;

                Task.Delay(100).ContinueWith(_ =>
                {
                    windowDispatcher.Invoke(SetTextBlocksColor_DPS_ToDefault, System.Windows.Threading.DispatcherPriority.Normal);
                }
                );

                Clipboard.SetText(TextBlock_DPS_Value.Text);
                lastLeftClick_TextBlock_DPS_Value = 0;
            }
            else
            {
                lastLeftClick_TextBlock_DPS_Value = timeNow;
            }
        }


        public void SetTextBlocksColor_DMG_ToDefault()
        {
            TextBlock_DMG.Foreground = defaultForeground;
            TextBlock_DMG_Value.Foreground = defaultForeground;
        }

        public void SetTextBlocksColor_Hits_ToDefault()
        {
            TextBlock_Hits.Foreground = defaultForeground;
            TextBlock_Hits_Value.Foreground = defaultForeground;
        }

        public void SetTextBlocksColor_MaxHitDmg_ToDefault()
        {
            TextBlock_MaxHitDmg.Foreground = defaultForeground;
            TextBlock_MaxHitDmg_Value.Foreground = defaultForeground;
        }

        public void SetTextBlocksColor_T_ToDefault()
        {
            TextBlock_T.Foreground = defaultForeground;
            TextBlock_T_Value.Foreground = defaultForeground;
        }

        public void SetTextBlocksColor_DPS_ToDefault()
        {
            TextBlock_DPS.Foreground = defaultForeground;
            TextBlock_DPS_Value.Foreground = defaultForeground;
        }

        private void ContextMenu_Item_CopyAll_Click(object sender, RoutedEventArgs e)
        {
            string t =
                TextBlock_TargetName.Text + Environment.NewLine +
                "dmg: " + TextBlock_DMG_Value.Text + " " + TextBlock_DMG_PercentValue.Text + Environment.NewLine +
                "t: " + TextBlock_T_Value.Text + Environment.NewLine +
                "dps: " + TextBlock_DPS_Value.Text + Environment.NewLine +
                "hits: " + TextBlock_Hits_Value.Text + Environment.NewLine +
                "max: " + TextBlock_MaxHitDmg_Value.Text;
            Clipboard.SetText(t);
        }

        private void ContextMenu_Item_CopyDmg_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_DMG_Value.Text);
        }

        private void ContextMenu_Item_CopyT_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_T_Value.Text);
        }

        private void ContextMenu_Item_CopyDps_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_DPS_Value.Text);
        }

        private void ContextMenu_Item_CopyHits_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_Hits_Value.Text);
        }

        private void ContextMenu_Item_CopyMaxHitDmg_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TextBlock_MaxHitDmg_Value.Text);
        }

        private void ContextMenu_Item_ResetStat_Click(object sender, RoutedEventArgs e)
        {
            ResetStatistics();
        }

        private void RefreshTargetNameWithAliasOverride()
        {
            if (currentTarget_OriginalName.Length < 1)
                return;

            if (UncorRTDPS_StaticSettings.DpsWindowSettings.EnableAliases && simpleAliasService_Mobs != null)
            {
                string alias = simpleAliasService_Mobs.GetAliasForName(currentTarget_OriginalName);
                if (alias != null)
                    TextBlock_TargetName.Text = alias;
                else
                    TextBlock_TargetName.Text = currentTarget_OriginalName;
            }
            else TextBlock_TargetName.Text = currentTarget_OriginalName;
        }

        private void SetVisualDataToZero()
        {
            TextBlock_DMG_Value.Text = "0";
            TextBlock_Hits_Value.Text = "0";
            TextBlock_MaxHitDmg_Value.Text = "0";
            TextBlock_T_Value.Text = "0";
            TextBlock_DPS_Value.Text = "0";
            TextBlock_DMG_PercentValue.Text = "";
        }

        private void ResetStatistics()
        {
            lastDamageModel?.AskForReset();
            SetVisualDataToZero();
        }

        public void SettingsWindowClosed_HandleEvent(object sender, EventArgs e)
        {
            RefreshTargetNameWithAliasOverride();
        }

        ~Panel_DamageInfo()
        {
            Dispose();
        }

        public void Dispose()
        {
            GlobalEvents.SettingsWindowClosed -= SettingsWindowClosed_HandleEvent;
        }

        public void OpenDamageChart()
        {
            if (lastDamageModel == null)
                return;
            DamageChartWindow damageChartWindow = new DamageChartWindow();
            damageChartWindow.Init(lastDamageModel.DamageSequence.Clone(), lastTarget == null ? commonTargetName : lastTarget.originalName);
            if (damageChartWindow.IsStartupPositionUnknown)
            {
                damageChartWindow.Top = UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionY;
                damageChartWindow.Left = UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionX;
            }
            damageChartWindow.Show();
        }

        public Target GetLastTarget()
        {
            return lastTarget;
        }

        private void ContextMenu_Item_OpenChart_Click(object sender, RoutedEventArgs e)
        {
            OpenDamageChart();
        }
    }
}
