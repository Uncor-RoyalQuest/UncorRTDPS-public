using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UncorRTDPS.DpsModels;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.Services;
using UncorRTDPS.Services.GlobalKeyPressListener;
using UncorRTDPS.Services.HotKeys;
using UncorRTDPS.Services.WindowSize;
using UncorRTDPS.UncorOCR;
using UncorRTDPS.UncorRTDPS_Windows.DpsPanel;
using UncorRTDPS.Util;

namespace UncorRTDPS.UncorRTDPS_Windows
{
    /// <summary>
    /// Interaction logic for StatsHoveringWindow.xaml
    /// </summary>
    public partial class StatsHoveringWindow : Window, IDPSWindowSettingsChangedListener, IDisposable
    {
        private readonly string uid = "StatsHoveringWindow_" + UniqueLongGenerator.GetUniqueId();
        private const int widthNheight = 12;
        private const int widthNheight2 = 14;
        private const int widthNheight3 = 10;
        private const int widthNheight_DamageHistory = 16;

        private Brush defaultForeground = new SolidColorBrush(Color.FromArgb(255, 171, 171, 167));
        private Brush selectedForeground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 55));


        private const string name = "StatsMainWindow";
        private WindowSizeService windowSizeService = null;
        public StatsHoveringWindow()
        {
            //Test.test();

            InitializeComponent();
            InitImages();
            InitLocaleText();

            Service service = ServicesContainer.GetService("windowSizeService");
            if (service != null && service is WindowSizeService)
            {
                windowSizeService = service as WindowSizeService;
                Size<double> s = windowSizeService.GetWindowPosition(name);
                if (s != null)
                {
                    this.Width = s.Width;
                }
            }

            if (UncorRTDPS_Config.GetConfigVal("prohibitTransparency") == "0")
            {
                this.AllowsTransparency = true;
            }

            windowDispatcher = Application.Current.Dispatcher;
            manager_DamageInfoSequence = new Manager_DamageInfoSequence(StackPanel_DamageInfoSequence);
            RefreshWindowOptions();
            UncorRTDPS_StaticSettings.RegisterDPSWindowSettingsChangedListener(this);

            GlobalEvents.SettingsWindowOpened += SettingsWindowOpened_HandleEvent;
            GlobalEvents.SettingsWindowClosed += SettingsWindowClosed_HandleEvent;

            GlobalEvents.HotkeysSettingsOpened += HotkeysSettingsOpened_HandleEvent;
            GlobalEvents.HotkeysSettingsClosed += HotkeysSettingsClosed_HandleEvent;

            RegisterHotkeys_Monitoring();
            RegisterHotkeys_WindowsOpenClose();
            RegisterHotkeys_DamageInfoManager();
        }

        public void InitImages()
        {
            Image_Status.Source = UncorRTDPS_StaticSettings.BiCircleGray;
            Image_Settings.Source = UncorRTDPS_StaticSettings.BiGears;

            Image_Status.Width = widthNheight;
            Image_Status.Height = widthNheight;

            Image_Settings.Width = widthNheight2;
            Image_Settings.Height = widthNheight2;

            Image_LockDragRTDPS.Source = UncorRTDPS_StaticSettings.BiLockLockedGray;
            Image_LockDragRTDPS.Width = widthNheight3;

            Image_CloseRTDPS.Source = UncorRTDPS_StaticSettings.BiWClose;
            Image_CloseRTDPS.Width = widthNheight2;

            Image_DamageHistory.Source = UncorRTDPS_StaticSettings.BiDamageHistory;
            Image_DamageHistory.Width = widthNheight_DamageHistory;
        }

        public void InitLocaleText()
        {
            StatS_Failures.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_Failures");
            StatS_Failures.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_Failures_tooltip");

            StatS_Losses.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_Losses");
            StatS_Losses.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_Losses_tooltip");

            StatA_RPS.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_RPS");
            StatA_RPS.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_RPS_tooltip");

            StatA_AVG_TIME.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_ART");
            StatA_AVG_TIME.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_ART_tooltip");

            StatA_ALoad.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_ALoad");
            StatA_ALoad.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_ALoad_tooltip");

            StatA_MLoad.Text = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_MLoad");
            StatA_MLoad.ToolTip = UncorRTDPS_Localization.GetLocaleGuiVal("guiOcrStat_MLoad_tooltip");
        }

        public bool IsHotKeysRegistered_DamageInfoManager { get; private set; } = false;
        private void RegisterHotkeys_DamageInfoManager()
        {
            if (IsHotKeysRegistered_DamageInfoManager)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotKeysStorageService hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;
            if (hotKeysStorageService == null)
                return;

            //Open First Non Common Detailed Damage
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_OpenFirstNonCommonDetailedDamage, () => manager_DamageInfoSequence?.ShowFirstNonCommonTargetDetailedDamage());


            IsHotKeysRegistered_DamageInfoManager = true;
        }

        private void UnregisterHotkeys_DamageInfoManager()
        {
            if (!IsHotKeysRegistered_DamageInfoManager)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_OpenFirstNonCommonDetailedDamage);


            IsHotKeysRegistered_DamageInfoManager = false;
        }

        public bool IsHotKeysRegistered_Monitoring { get; private set; } = false;
        private void RegisterHotkeys_Monitoring()
        {
            if (IsHotKeysRegistered_Monitoring)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotKeysStorageService hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;
            if (hotKeysStorageService == null)
                return;

            //StartMonitoring
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_StartMonitoring, () => StartRTDPS());
            //StopMonitoring
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_StopMonitoring, () => StopRTDPS());
            //ToggleMonitoring
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_ToggleMonitoring, () => ToggleRTDPS());


            IsHotKeysRegistered_Monitoring = true;
        }

        private void UnregisterHotkeys_Monitoring()
        {
            if (!IsHotKeysRegistered_Monitoring)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_StartMonitoring);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_StopMonitoring);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_ToggleMonitoring);

            IsHotKeysRegistered_Monitoring = false;
        }

        public bool IsHotKeysRegistered_WindowsOpenClose { get; private set; } = false;
        private void RegisterHotkeys_WindowsOpenClose()
        {
            if (IsHotKeysRegistered_WindowsOpenClose)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotKeysStorageService hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;
            if (hotKeysStorageService == null)
                return;

            //Open Settings
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_OpenSettings, () => OpenSettingsWindow());
            //Close Settings
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_CloseSettings, () => CloseSettingsWindow());
            //Toggle Settings
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_ToggleSettings, () => ToggleSettingsWindow());

            //Open DamageHistoryWindow
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_OpenDamageHistory, () => OpenDamageHistoryWindow());
            //Close DamageHistoryWindow
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_CloseDamageHistory, () => CloseDamageHistoryWindow());
            //Toggle DamageHistoryWindow
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_ToggleDamageHistory, () => ToggleDamageHistoryWindow());

            //Close all except main
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_CloseAllButMain, () => GlobalEvents.InvokeCloseAllWindowsButMain(this, null));
            //Close all charts
            HotkeyActionRegistrator.RegisterActionToHotKey(globalKeyPressService, hotKeysStorageService, uid, CombinationNames.CombinationName_CloseAllCharts, () => GlobalEvents.InvokeCloseAllWindowsCharts(this, null));

            IsHotKeysRegistered_WindowsOpenClose = true;
        }

        private void UnregisterHotkeys_WindowsOpenClose()
        {
            if (!IsHotKeysRegistered_WindowsOpenClose)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            //Settings window open/close/toggle
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_OpenSettings);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_CloseSettings);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_ToggleSettings);

            //DamageHistoryWindow open/close/toggle
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_OpenDamageHistory);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_CloseDamageHistory);
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_ToggleDamageHistory);

            //Close all except main
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_CloseAllButMain);

            //Close all charts
            HotkeyActionRegistrator.UnregisterActionFromHotKey(globalKeyPressService, uid, CombinationNames.CombinationName_CloseAllCharts);

            IsHotKeysRegistered_WindowsOpenClose = false;
        }

        private bool IsRTDPSRunning()
        {
            if (screenMonitoring == null)
                return false;
            return screenMonitoring.IsWorking;
        }

        private void ImageButton_Status_Click(object sender, RoutedEventArgs e)
        {
            ToggleRTDPS();
        }


        private StatsSettingsHoveringWindow statsSettingsWindow = null;
        private void OpenSettingsWindow()
        {
            if (statsSettingsWindow != null)
                return;

            StatsSettingsHoveringWindow statsSettingsHoveringWindow = new StatsSettingsHoveringWindow();
            if (statsSettingsHoveringWindow.IsStartupPositionUnknown)
            {
                statsSettingsHoveringWindow.Left = this.Left;
                statsSettingsHoveringWindow.Top = this.Top;
            }
            this.statsSettingsWindow = statsSettingsHoveringWindow;
            statsSettingsHoveringWindow.ShowDialog();
            statsSettingsHoveringWindow.Dispose();

            GC.Collect();
            this.statsSettingsWindow = null;
        }

        private void CloseSettingsWindow()
        {
            if (statsSettingsWindow == null)
                return;
            statsSettingsWindow.Close();
            statsSettingsWindow = null;
        }

        private void ToggleSettingsWindow()
        {
            if (statsSettingsWindow == null)
            {
                OpenSettingsWindow();
            }
            else
            {
                CloseSettingsWindow();
            }
        }

        private void ImageButton_Settings_Click(object sender, RoutedEventArgs e)
        {
            OpenSettingsWindow();

        }

        private void ImageButton_Close_Click(object sender, RoutedEventArgs e)
        {
            //stop all that running
            if (IsRTDPSRunning())
            {
                int tryStopRes = TryStopRTDPS();

                StopUpdateStat_RTDPS();

                Image_Status.Source = UncorRTDPS_StaticSettings.BiCircleGray;
            }

            Dispose();

            //close
            this.Close();
        }

        public void StartRTDPS()
        {
            if (!IsRTDPSRunning())
            {
                ToggleRTDPS();
            }
        }

        public void StopRTDPS()
        {
            if (IsRTDPSRunning())
            {
                ToggleRTDPS();
            }
        }

        public void ToggleRTDPS()
        {
            if (IsRTDPSRunning())
            {
                int tryStopRes = TryStopRTDPS();
                if (tryStopRes == -1)
                    return;

                StopUpdateStat_RTDPS();

                Image_Status.Source = UncorRTDPS_StaticSettings.BiCircleGray;
            }
            else
            {
                int tryStartRes = TryStartRTDPS();
                if (tryStartRes == -1)
                    return;

                StartUpdateStat_RTDPS();

                Image_Status.Source = UncorRTDPS_StaticSettings.BiCircleGreen;
            }
        }

        private TimerCallback tcbStatsUpd_RTDPS;
        private Timer timerStatsUpd_RTDPS;
        private System.Windows.Threading.Dispatcher windowDispatcher;
        public void StartUpdateStat_RTDPS()
        {
            if (timerStatsUpd_RTDPS != null)
            {
                timerStatsUpd_RTDPS.Dispose();
            }

            tcbStatsUpd_RTDPS = new TimerCallback(TimerCallbackFunc_RTDPS);
            long timerStatsUpdPeriodMS_RTDPS = UncorRTDPS_StaticSettings.DpsWindowSettings.VisualRefreshDelay;
            timerStatsUpd_RTDPS = new Timer(tcbStatsUpd_RTDPS, 0, timerStatsUpdPeriodMS_RTDPS, timerStatsUpdPeriodMS_RTDPS);
        }

        public void TimerCallbackFunc_RTDPS(object o)
        {
            windowDispatcher.Invoke(TimerUpdateFuntion_RTDPS, System.Windows.Threading.DispatcherPriority.Normal);
        }


        /// <summary>
        /// 0 - maximum priority
        /// 1 - all targets
        /// </summary>
        private int damageViewMode = 0;

        private bool ocrStatEnabled = true;

        public void TimerUpdateFuntion_RTDPS()
        {
            if (damagePoolManager == null)
                return;



            //Panel_DamageInfo_Test1.updateViewData(dpsModel.commonTarget_DamageModel, dpsModel.commonTarget_DamageModel.target);
            switch (damageViewMode)
            {
                case 0:
                    UpdateDpsInfo_DamageViewMode_0();
                    break;
                case 1:
                    UpdateDpsInfo_DamageViewMode_1();
                    break;
            }

            //OCR STAT

            if (ocrStatEnabled)
            {
                StatS_Failures_Value.Text = String.Format("{0:0.##}", damageOCR.FailureRecognitionProportion * 100);
                StatS_Losses_Value.Text = String.Format("{0:0.##}", damageOCR.LossesDamageConcatProportion * 100);

                StatA_RPS_Value.Text = String.Format("{0:0.##}", screenMonitoring.Statistics_RefreshRate.GetRefreshRate());
                StatA_AVG_TIME_Value.Text = String.Format("{0:0.##}", screenMonitoring.Statistics_TimeConsumptionPerEvent.GetAvgDuration());

                StatA_ALoad_Value.Text = String.Format("{0:0.##}", damageOCR.AverageDamageLoad * 100);
                StatA_MLoad_Value.Text = String.Format("{0:0.##}", damageOCR.MaxDamageLoad * 100);

            }

        }

        private Manager_DamageInfoSequence manager_DamageInfoSequence;
        public void UpdateDpsInfo_DamageViewMode_0()
        {
            //try get active boss
            DamageModel dt_dm = damagePoolManager.GetMostActiveBoss(5000);
            if (dt_dm == null)
            {
                //try get active elite
                dt_dm = damagePoolManager.GetMostActiveElite(5000);
                if (dt_dm == null)
                {
                    //just get common
                    dt_dm = damagePoolManager.CommonTarget;
                }
            }

            manager_DamageInfoSequence.UpdateDamageInfo(dt_dm);
        }


        public void InsertionSortByShift(DamageModel[] arr, int n)
        {
            DamageModel key;
            long keyVal;
            int i, j;
            for (i = 1; i < n; i++)
            {
                key = arr[i];
                keyVal = key.TimeLast;
                j = i - 1;

                while (j >= 0 && arr[j].TimeLast < keyVal)
                {
                    arr[j + 1] = arr[j];
                    j = j - 1;
                }
                arr[j + 1] = key;
            }
        }


        private int bossesLimit_damageViewMode_1 = 2;
        private int elitesLimit_damageViewMode_1 = 2;
        private bool showCommonMobsDps_damageViewMode_1 = true;

        public void UpdateDpsInfo_DamageViewMode_1()
        {
            int posPanel = 0;
            DamageModel damageModel;

            //------------BOSSES----------
            int bossesLen;
            DamageModel[] bossesArr;
            (bossesLen, bossesArr) = damagePoolManager.GetActiveBosses_BufferedArray();

            //sort bosses array by the last dmg time
            if (bossesLen > 1)
                InsertionSortByShift(bossesArr, bossesLen);

            //add bosses to corresponding pannels depending on
            for (int i = 0; i < bossesLimit_damageViewMode_1 && i < bossesLen; i++)
            {
                manager_DamageInfoSequence.UpdateDamageInfo(bossesArr[i], posPanel);
                posPanel += 1;
            }


            //-----------ELITES-----------
            int elitesLen;
            DamageModel[] elitesArr;
            (elitesLen, elitesArr) = damagePoolManager.GetActiveElites_BufferedArray();

            //sort elits array by the last dmg time
            if (elitesLen > 1)
                InsertionSortByShift(elitesArr, elitesLen);

            //add elites to corresponding pannels
            for (int i = 0; i < elitesLimit_damageViewMode_1 && i < elitesLen; i++)
            {
                manager_DamageInfoSequence.UpdateDamageInfo(elitesArr[i], posPanel);
                posPanel += 1;
            }

            //----------COMMON------------
            damageModel = damagePoolManager.CommonTarget;
            if (damageModel.IsActive && showCommonMobsDps_damageViewMode_1)
            {
                manager_DamageInfoSequence.UpdateDamageInfo(damageModel, posPanel);
                posPanel += 1;
            }

            //visibility setup
            manager_DamageInfoSequence.SetPanelsVisibility(0, posPanel, Visibility.Visible);
            manager_DamageInfoSequence.SetPanelsVisibility(posPanel, 500, Visibility.Collapsed);

        }

        public void StopUpdateStat_RTDPS()
        {
            if (timerStatsUpd_RTDPS != null)
            {
                timerStatsUpd_RTDPS.Dispose();
            }
        }


        //

        private DamageTarget_PoolManager damagePoolManager;
        private DamageOCR_Target_v0 damageOCR;
        private MonitorScreenArea screenMonitoring;
        public int TryStartRTDPS()
        {
            if (damagePoolManager == null)
            {
                string distanceMethod = UncorRTDPS_Config.GetConfigVal("distanceMethod");
                DamageTarget_PoolManager.SearchTargetNameMethod searchTargetNameMethod;
                if (distanceMethod.Equals("ham"))
                {
                    searchTargetNameMethod = DamageTarget_PoolManager.SearchTargetNameMethod.HammingDistance;
                }
                else if (distanceMethod.Equals("lev"))
                {
                    searchTargetNameMethod = DamageTarget_PoolManager.SearchTargetNameMethod.LevenshteinDistance;
                }
                else
                {
                    searchTargetNameMethod = DamageTarget_PoolManager.SearchTargetNameMethod.HammingDistance;
                }
                damagePoolManager = new DamageTarget_PoolManager(searchTargetNameMethod);
            }
            damagePoolManager.ApplySettings(UncorRTDPS_StaticSettings.DpsModelSettings);

            if (damageOCR == null)
            {
                //damageOCR = new DamageOCR_LameBeta();
                //damageOCR.registerDamageListener(dpsModel);
                damageOCR = new DamageOCR_Target_v0();
                damageOCR.RegisterTargetDamageListener(damagePoolManager);
            }
            damageOCR.InitEngine(UncorRTDPS_StaticSettings.OcrSettings.Lang);
            //damageOCR.applySettings(UncorRTDPS_StaticSettings.ocrSettings);

            //
            if (UncorRTDPS_StaticSettings.Supplementary_Parameters_DamageOCR == null ||
                !UncorRTDPS_StaticSettings.Supplementary_Parameters_DamageOCR.CheckParamsValid())
                return -1;

            damageOCR.UpdateSettingsOCR(UncorRTDPS_StaticSettings.OcrSettings, UncorRTDPS_StaticSettings.Supplementary_Parameters_DamageOCR);

            //MonitorScreenArea msa = new MonitorScreenArea(500, 1100, 50, 122);
            if (screenMonitoring == null)
            {
                screenMonitoring = new MonitorScreenArea();
                screenMonitoring.SubscribeBitmapUpdate(damageOCR);
            }
            screenMonitoring.SetAreaOfMonitoring(UncorRTDPS_StaticSettings.SelectedAreaRTDPS);

            return screenMonitoring.StartMonitoringThread();
        }

        public int TryStopRTDPS()
        {
            return screenMonitoring.StopMonitoringThread();
        }

        public void FireEventDPSWindowSettingsChanged()
        {
            RefreshWindowOptions();
        }

        public void RefreshWindowOptions()
        {
            if (this.AllowsTransparency)
            {
                this.Background.Opacity = UncorRTDPS_StaticSettings.DpsWindowSettings.Opacity;
                Grid_StatS.Opacity = UncorRTDPS_StaticSettings.DpsWindowSettings.Opacity;
                Grid_StatA.Opacity = UncorRTDPS_StaticSettings.DpsWindowSettings.Opacity;
            }
            this.FontSize = UncorRTDPS_StaticSettings.DpsWindowSettings.FontSize;
            Grid_StatS.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat ? Visibility.Visible : Visibility.Collapsed;
            Grid_StatA.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat ? Visibility.Visible : Visibility.Collapsed;
            this.Left = UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionX;
            this.Top = UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionY;

            this.damageViewMode = UncorRTDPS_StaticSettings.DpsWindowSettings.DpsViewMode;

            //dps visibility rows
            manager_DamageInfoSequence.UpdatePanelsVisibilityParameters();

            //ocr stats visibility rows
            StatS_Failures.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Failures ? Visibility.Visible : Visibility.Collapsed;
            StatS_Failures_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Failures ? Visibility.Visible : Visibility.Collapsed;
            StatS_Failures_PercentChar.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Failures ? Visibility.Visible : Visibility.Collapsed;

            StatS_Losses.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Losses ? Visibility.Visible : Visibility.Collapsed;
            StatS_Losses_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Losses ? Visibility.Visible : Visibility.Collapsed;
            StatS_Losses_PercentChar.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_Losses ? Visibility.Visible : Visibility.Collapsed;

            StatA_RPS.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_RPS ? Visibility.Visible : Visibility.Collapsed;
            StatA_RPS_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_RPS ? Visibility.Visible : Visibility.Collapsed;

            StatA_AVG_TIME.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ART ? Visibility.Visible : Visibility.Collapsed;
            StatA_AVG_TIME_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ART ? Visibility.Visible : Visibility.Collapsed;

            StatA_ALoad.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ALoad ? Visibility.Visible : Visibility.Collapsed;
            StatA_ALoad_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ALoad ? Visibility.Visible : Visibility.Collapsed;
            StatA_ALoad_PercentChar.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ALoad ? Visibility.Visible : Visibility.Collapsed;

            StatA_MLoad.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_MLoad ? Visibility.Visible : Visibility.Collapsed;
            StatA_MLoad_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_MLoad ? Visibility.Visible : Visibility.Collapsed;
            StatA_MLoad_PercentChar.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_MLoad ? Visibility.Visible : Visibility.Collapsed;


            bossesLimit_damageViewMode_1 = UncorRTDPS_StaticSettings.DpsWindowSettings.BossesLimit_mode_1;
            elitesLimit_damageViewMode_1 = UncorRTDPS_StaticSettings.DpsWindowSettings.ElitesLimit_mode_1;
            showCommonMobsDps_damageViewMode_1 = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowCommonMobsDps_mode_1;
        }

        public void Dispose()
        {
            UncorRTDPS_StaticSettings.UnregisterDPSWindowSettingsChangedListener(this);
            Image_CloseRTDPS.Source = null;
            Image_LockDragRTDPS.Source = null;
            Image_Settings.Source = null;
            Image_Status.Source = null;

            //
            StatS_Failures.Text = null;
            StatS_Failures.ToolTip = null;

            StatS_Losses.Text = null;
            StatS_Losses.ToolTip = null;

            StatA_RPS.Text = null;
            StatA_RPS.ToolTip = null;

            StatA_AVG_TIME.Text = null;
            StatA_AVG_TIME.ToolTip = null;

            StatA_ALoad.Text = null;
            StatA_ALoad.ToolTip = null;

            StatA_MLoad.Text = null;
            StatA_MLoad.ToolTip = null;

            GlobalEvents.SettingsWindowOpened -= SettingsWindowOpened_HandleEvent;
            GlobalEvents.SettingsWindowClosed -= SettingsWindowClosed_HandleEvent;

            GlobalEvents.HotkeysSettingsOpened -= HotkeysSettingsOpened_HandleEvent;
            GlobalEvents.HotkeysSettingsClosed -= HotkeysSettingsClosed_HandleEvent;

            UnregisterHotkeys_Monitoring();
            UnregisterHotkeys_WindowsOpenClose();
            UnregisterHotkeys_DamageInfoManager();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (allowedDragThisWindow)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            }
        }

        private bool allowedDragThisWindow = false;
        private void ImageButton_DragRTDPS_Click(object sender, RoutedEventArgs e)
        {
            allowedDragThisWindow = !allowedDragThisWindow;
            if (allowedDragThisWindow)
            {
                Image_LockDragRTDPS.Source = UncorRTDPS_StaticSettings.BiLockGray;
                ResizeMode = ResizeMode.CanResizeWithGrip;
            }
            else
            {
                Image_LockDragRTDPS.Source = UncorRTDPS_StaticSettings.BiLockLockedGray;
                this.ResizeMode = ResizeMode.NoResize;
                this.SizeToContent = SizeToContent.Height;
            }
        }

        public void SaveCurrentWindowPosition()
        {
            int currentX = (int)this.Left;
            int currentY = (int)this.Top;
            if (currentX != UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionX ||
                currentY != UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionY)
            {
                DPSWindowSettings s = new DPSWindowSettings(UncorRTDPS_StaticSettings.DpsWindowSettings);
                s.ScreenPositionX = currentX;
                s.ScreenPositionY = currentY;
                UncorRTDPS_StaticSettings.UpdateDPSWindowSettings(s);
            }
        }

        public void ForceSaveCurrentWindowPositionToConfigFile()
        {
            int currentX = (int)this.Left;
            int currentY = (int)this.Top;
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_X", SInt.ToString(currentX));
            UncorRTDPS_Config.UpdateConfigVal("dpsWindow_Y", SInt.ToString(currentY));
            UncorRTDPS_Config.SaveConfigs();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowSizeService.UpdateWindowSize(name, new Size<double>(this.ActualWidth, this.ActualHeight));
            ForceSaveCurrentWindowPositionToConfigFile();
            Dispose();
            ServicesContainer.CloseServicesContainer();
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            int currentX = (int)this.Left;
            int currentY = (int)this.Top;
            UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionX = currentX;
            UncorRTDPS_StaticSettings.DpsWindowSettings.ScreenPositionY = currentY;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private Services.DamageHistory.DamageHistoryWindow _damageHistoryWindow = null;
        private void OpenDamageHistoryWindow()
        {
            if (_damageHistoryWindow != null)
                return;
            Services.DamageHistory.DamageHistoryWindow damageHistoryWindow = new Services.DamageHistory.DamageHistoryWindow();
            if (damageHistoryWindow.IsStartupPositionUnknown)
            {
                damageHistoryWindow.Top = this.Top;
                damageHistoryWindow.Left = this.Left;
            }
            _damageHistoryWindow = damageHistoryWindow;
            damageHistoryWindow.ShowDialog();
            _damageHistoryWindow = null;
        }

        private void CloseDamageHistoryWindow()
        {
            if (_damageHistoryWindow == null)
                return;
            _damageHistoryWindow.Close();
            _damageHistoryWindow = null;
        }

        private void ToggleDamageHistoryWindow()
        {
            if (_damageHistoryWindow == null)
            {
                OpenDamageHistoryWindow();
            }
            else
            {
                CloseDamageHistoryWindow();
            }
        }

        private void ImageButton_History_Click(object sender, RoutedEventArgs e)
        {
            OpenDamageHistoryWindow();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Application.Current.Shutdown();
        }

        private bool resumeRTDPSAfterSettingsIfWasRunning = false;
        private void SettingsWindowOpened_HandleEvent(object sender, EventArgs e)
        {
            UnregisterHotkeys_Monitoring();

            resumeRTDPSAfterSettingsIfWasRunning = false;
            if (IsRTDPSRunning())
            {
                resumeRTDPSAfterSettingsIfWasRunning = true;
                ToggleRTDPS();
            }
        }

        private void SettingsWindowClosed_HandleEvent(object sender, EventArgs e)
        {
            RegisterHotkeys_Monitoring();

            if (resumeRTDPSAfterSettingsIfWasRunning)
                ToggleRTDPS();
            resumeRTDPSAfterSettingsIfWasRunning = false;
        }

        private void HotkeysSettingsOpened_HandleEvent(object sender, EventArgs e)
        {
            UnregisterHotkeys_WindowsOpenClose();
            manager_DamageInfoSequence.UnregisterHotkeysOnPanels();
            UnregisterHotkeys_DamageInfoManager();
        }

        private void HotkeysSettingsClosed_HandleEvent(object sender, EventArgs e)
        {
            RegisterHotkeys_WindowsOpenClose();
            manager_DamageInfoSequence.RefreshHotkeysOnPanels();
            RegisterHotkeys_DamageInfoManager();
        }
    }
}
