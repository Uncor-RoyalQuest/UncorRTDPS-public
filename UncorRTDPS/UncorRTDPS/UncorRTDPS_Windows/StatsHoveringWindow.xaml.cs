using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UncorRTDPS.DpsModels;
using UncorRTDPS.RTDPS_Settings;
using UncorRTDPS.UncorRTDPS_Windows.DpsPanel;
using UncorRTDPS.Util;
using UncorRTDPS.Services;
using UncorRTDPS.Services.GlobalKeyPressListener;
using UncorRTDPS.Services.HotKeys;
using UncorRTDPS.UncorOCR;

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

        private Brush defaultForeground = new SolidColorBrush(Color.FromArgb(255, 171, 171, 167));
        private Brush selectedForeground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 55));

        public StatsHoveringWindow()
        {
            //Test.test();

            InitializeComponent();
            InitImages();
            InitLocaleText();

            if (UncorRTDPS_Config.getConfigVal("prohibitTransparency") == "0")
            {
                this.AllowsTransparency = true;
            }

            windowDispatcher = Application.Current.Dispatcher;
            optimizer_DamageInfoSequence = new Optimizer_DamageInfoSequence(StackPanel_DamageInfoSequence);
            RefreshWindowOptions();
            UncorRTDPS_StaticSettings.RegisterDPSWindowSettingsChangedListener(this);

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

        public bool IsHotKeysRegistered { get; private set; } = false;
        private void RegisterHotkeys()
        {
            if (IsHotKeysRegistered)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            HotKeysStorageService hotKeysStorageService = ServicesContainer.GetService("hotKeysStorageService") as HotKeysStorageService;
            if (hotKeysStorageService == null)
                return;

            
            //---
            //create hotkey StartMonitoring
            HotKeyCombination hotKeyCombination_StartMonitoring = hotKeysStorageService.GetCombinationForName("startMonitoring");
            if (hotKeyCombination_StartMonitoring != null)
            {

                KeyPressSequence keyPressSequence = new KeyPressSequence(hotKeyCombination_StartMonitoring);
                keyPressSequence.DelegatedAction = () =>
                {
                    StartRTDPS();
                };

                //register hotkey StartMonitoring
                if (keyPressSequence.SequenceLength > 0)
                {
                    globalKeyPressService.RegisterEventHandler_OnKeyPressed(uid + "startMonitoring", keyPressSequence);
                }
            }
            
            //---
            //create hotkey StopMonitoring
            HotKeyCombination hotKeyCombination_StopMonitoring = hotKeysStorageService.GetCombinationForName("stopMonitoring");
            if (hotKeyCombination_StopMonitoring != null)
            {

                KeyPressSequence keyPressSequence = new KeyPressSequence(hotKeyCombination_StopMonitoring);
                keyPressSequence.DelegatedAction = () =>
                {
                    StopRTDPS();
                };

                //register hotkey StopMonitoring
                if (keyPressSequence.SequenceLength > 0)
                {
                    globalKeyPressService.RegisterEventHandler_OnKeyPressed(uid + "stopMonitoring", keyPressSequence);
                }
            }
            
            //---
            //create hotkey ToggleMonitoring
            HotKeyCombination hotKeyCombination_ToggleMonitoring = hotKeysStorageService.GetCombinationForName("toggleMonitoring");
            if (hotKeyCombination_ToggleMonitoring != null)
            {

                KeyPressSequence keyPressSequence = new KeyPressSequence(hotKeyCombination_ToggleMonitoring);
                keyPressSequence.DelegatedAction = () =>
                {
                    ToggleRTDPS();
                };

                //register hotkey ToggleMonitoring
                if (keyPressSequence.SequenceLength > 0)
                {
                    globalKeyPressService.RegisterEventHandler_OnKeyPressed(uid + "toggleMonitoring", keyPressSequence);
                }
            }
            
            IsHotKeysRegistered = true;
        }

        private void UnregisterHotkeys()
        {
            if (!IsHotKeysRegistered)
                return;

            GlobalKeyPressService globalKeyPressService = ServicesContainer.GetService("globalKeyPressService") as GlobalKeyPressService;
            if (globalKeyPressService == null)
                return;

            globalKeyPressService.UnregisterEventHandler_OnKeyPressed(uid + "startMonitoring");
            globalKeyPressService.UnregisterEventHandler_OnKeyPressed(uid + "stopMonitoring");
            globalKeyPressService.UnregisterEventHandler_OnKeyPressed(uid + "toggleMonitoring");

            IsHotKeysRegistered = false;
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

        private void ImageButton_Settings_Click(object sender, RoutedEventArgs e)
        {
            UnregisterHotkeys();

            bool resumeRTDPSAfterSettingsIfWasRunning = false;
            if (IsRTDPSRunning())
            {
                resumeRTDPSAfterSettingsIfWasRunning = true;
                ToggleRTDPS();
            }

            StatsSettingsHoveringWindow statsSettingsHoveringWindow = new StatsSettingsHoveringWindow();
            if (statsSettingsHoveringWindow.IsStartupPositionUnknown)
            {
                statsSettingsHoveringWindow.Left = this.Left;
                statsSettingsHoveringWindow.Top = this.Top;
            }
            statsSettingsHoveringWindow.ShowDialog();
            statsSettingsHoveringWindow.Dispose();

            GlobalEvents.InvokeSettingsWindowClosed(this,null);

            RegisterHotkeys();
            optimizer_DamageInfoSequence.RefreshHotkeysOnPanels();

            GC.Collect();
            if (resumeRTDPSAfterSettingsIfWasRunning)
                ToggleRTDPS();

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

            bossesToShow_SortBuffer = new DamageTarget_DamageModel[UncorRTDPS_StaticSettings.DpsWindowSettings.BossesLimit_mode_1];
            elitesToShow_SortBuffer = new DamageTarget_DamageModel[UncorRTDPS_StaticSettings.DpsWindowSettings.ElitesLimit_mode_1];

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
            if (dpsModel == null)
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

        private Optimizer_DamageInfoSequence optimizer_DamageInfoSequence;
        public void UpdateDpsInfo_DamageViewMode_0()
        {
            //try get active boss
            DamageTarget_DamageModel dt_dm = dpsModel.GetMostActiveBoss(5000);
            if (dt_dm == null)
            {
                //try get active elite
                dt_dm = dpsModel.GetMostActiveElite(5000);
                if (dt_dm == null)
                {
                    //just get common
                    dt_dm = dpsModel.commonTarget_DamageModel;
                }
            }

            optimizer_DamageInfoSequence.UpdateDamageInfo(dt_dm);
        }


        public static void InsertionSortByShift(DamageTarget_DamageModel[] arr, int n)
        {
            DamageTarget_DamageModel key;
            long keyVal;
            int i, j;
            for (i = 1; i < n; i++)
            {
                key = arr[i];
                keyVal = key.GetLastDamageTime();
                j = i - 1;

                while (j >= 0 && arr[j].GetLastDamageTime() < keyVal)
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

        private DamageTarget_DamageModel[] bossesToShow_SortBuffer;
        private DamageTarget_DamageModel[] elitesToShow_SortBuffer;

        public void UpdateDpsInfo_DamageViewMode_1()
        {
            int posPanel = 0;
            DamageTarget_DamageModel dt_dm;

            //BOSSES

            //adjust buffer array size
            if (bossesToShow_SortBuffer.Length < dpsModel.activePool_Bosses_CurrentLength)
                bossesToShow_SortBuffer = new DamageTarget_DamageModel[dpsModel.activePool_Bosses_CurrentLength + (dpsModel.activePool_Bosses_CurrentLength / 2)];
            
            //iter through all active bosses and add them to array
            int countBosses = 0;
            for (int i = 0; i < dpsModel.activePool_Bosses_CurrentLength; i++)
            {
                dt_dm = dpsModel.activePool_Bosses[i];
                if (dt_dm.IsActive)
                {
                    bossesToShow_SortBuffer[countBosses] = dt_dm;
                    countBosses += 1;
                }
            }

            //sort bosses array by the last dmg time
            InsertionSortByShift(bossesToShow_SortBuffer, countBosses);

            //add bosses to corresponding pannels depending on
            for (int i = 0; i < bossesLimit_damageViewMode_1 && i < countBosses; i++)
            {
                optimizer_DamageInfoSequence.UpdateDamageInfo(bossesToShow_SortBuffer[i], posPanel);
                posPanel += 1;
            }

            //ELITES
            
            //adjust buffer array size
            if (elitesToShow_SortBuffer.Length < dpsModel.activePool_Elites_CurrentLength)
                elitesToShow_SortBuffer = new DamageTarget_DamageModel[dpsModel.activePool_Elites_CurrentLength + (dpsModel.activePool_Elites_CurrentLength / 2)];
            
            //iter through active elites and add them to array
            int countElites = 0;
            for (int i = 0; i < dpsModel.activePool_Elites_CurrentLength; i++)
            {
                dt_dm = dpsModel.activePool_Elites[i];
                if (dt_dm.IsActive)
                {
                    elitesToShow_SortBuffer[countElites] = dt_dm;
                    countElites += 1;
                }
            }

            //sort bosses array by the last dmg time
            InsertionSortByShift(elitesToShow_SortBuffer, countElites);

            //add bosses to corresponding pannels
            for (int i = 0; i < elitesLimit_damageViewMode_1 && i < countElites; i++)
            {
                optimizer_DamageInfoSequence.UpdateDamageInfo(elitesToShow_SortBuffer[i], posPanel);
                posPanel += 1;
            }



            //COMMON
            dt_dm = dpsModel.commonTarget_DamageModel;
            if (dt_dm.IsActive && showCommonMobsDps_damageViewMode_1)
            {
                optimizer_DamageInfoSequence.UpdateDamageInfo(dt_dm, posPanel);
                posPanel += 1;
            }

            //visibility setup
            optimizer_DamageInfoSequence.SetPanelsVisibility(0, posPanel, Visibility.Visible);
            optimizer_DamageInfoSequence.SetPanelsVisibility(posPanel, 500, Visibility.Collapsed);

        }

        public void StopUpdateStat_RTDPS()
        {
            if (timerStatsUpd_RTDPS != null)
            {
                timerStatsUpd_RTDPS.Dispose();
            }
        }


        //

        private DamageTarget_PoolManager dpsModel;
        private DamageOCR_Target_v0 damageOCR;
        private MonitorScreenArea screenMonitoring;
        public int TryStartRTDPS()
        {
            if (dpsModel==null)
            {
                DpsModels.TargetsDictionary.TargetsDictionary.LoadDictionary(UncorRTDPS_StaticSettings.ResourcesPath, UncorRTDPS_StaticSettings.OcrSettings.Lang);

                string distanceMethod = UncorRTDPS_Config.getConfigVal("distanceMethod");
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
                dpsModel = new DamageTarget_PoolManager(searchTargetNameMethod);
            }
            dpsModel.ApplySettings(UncorRTDPS_StaticSettings.DpsModelSettings);

            if (damageOCR == null)
            {
                //damageOCR = new DamageOCR_LameBeta();
                //damageOCR.registerDamageListener(dpsModel);
                damageOCR = new DamageOCR_Target_v0();
                damageOCR.RegisterTargetDamageListener(dpsModel);
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
            optimizer_DamageInfoSequence.UpdatePanelsVisibilityParameters();

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
            StatA_ALoad_Value.Visibility = UncorRTDPS_StaticSettings.DpsWindowSettings.ShowOcrStat_ALoad? Visibility.Visible: Visibility.Collapsed;
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
            UnregisterHotkeys();
            ForceSaveCurrentWindowPositionToConfigFile();
            Services.ServicesContainer.CloseServicesContainer();
            Dispose();
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
            RegisterHotkeys();
        }
    }
}
