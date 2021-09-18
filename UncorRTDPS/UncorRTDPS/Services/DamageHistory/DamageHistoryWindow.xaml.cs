using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UncorRTDPS.DamageCharts;
using UncorRTDPS.DpsModels;
using UncorRTDPS.Services.WindowSize;
using UncorRTDPS.UncorRTDPS_Windows.EventAware;

namespace UncorRTDPS.Services.DamageHistory
{
    /// <summary>
    /// Interaction logic for DamageHistoryWindow.xaml
    /// </summary>
    public partial class DamageHistoryWindow : Window, IDisposable, ICloseAllWindowsButMainAware
    {

        private EntityViewModel entityViewModel = new EntityViewModel();
        private List<DamageModel> listDamageModel = null;

        private const string name = "damageHistoryWindow";
        private WindowPositionService windowPositionService = null;
        private WindowSizeService windowSizeService = null;
        public bool IsStartupPositionUnknown { get; set; } = true;

        public DamageHistoryWindow()
        {
            InitializeComponent();
            InitLocalization();

            (this as ICloseAllWindowsButMainAware).Register_CloseAllWindowsButMainAware();

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

            service = ServicesContainer.GetService("windowSizeService");
            if (service != null && service is WindowSizeService)
            {
                windowSizeService = service as WindowSizeService;
                Size<double> s = windowSizeService.GetWindowPosition(name);
                if (s != null)
                {
                    this.Width = s.Width;
                    this.Height = s.Height;
                }
            }

            listDamageModel = (ServicesContainer.GetService("recentDamage") as RecentDamage).GetClonedRecentDamage();
            entityViewModel.LoadEntities(listDamageModel);
            DataGrid_Entities.DataContext = entityViewModel;

            /*
            AddFakeEntityToDatagrid("Архон", 123, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Деструктор", 1234, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Деструктор", 12345, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("П'атаг", 543, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("П'атаг", 5437, DpsModels.TargetsDictionary.TargetType.Boss);
            AddFakeEntityToDatagrid("Пламярык", 54793, DpsModels.TargetsDictionary.TargetType.Elite);
            AddFakeEntityToDatagrid("Пламярык", 50993, DpsModels.TargetsDictionary.TargetType.Elite);
            AddFakeEntityToDatagrid("Пламярык", 9603, DpsModels.TargetsDictionary.TargetType.Elite);
            */
        }

        private void InitLocalization()
        {
            TextBlock_Header_RecentDamage_Name.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Name");
            TextBlock_Header_RecentDamage_Damage.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Damage");
            TextBlock_Header_RecentDamage_Dps.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Dps");
            TextBlock_Header_RecentDamage_Hits.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Hits");
            TextBlock_Header_RecentDamage_MaxHitDmg.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_MaxHitDmg");
            TextBlock_Header_RecentDamage_Time.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Time");
            TextBlock_Header_RecentDamage_TimeStart.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeStart");
            TextBlock_Header_RecentDamage_TimeEnd.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_TimeEnd");
            TextBlock_Header_RecentDamage_Details.Text = RTDPS_Settings.UncorRTDPS_Localization.GetLocaleGuiVal("guiGridHeader_Details");

        }

        /*
        private void AddFakeEntityToDatagrid(string name,int seed, DpsModels.TargetsDictionary.TargetType mobType)
        {
            Random r = new Random(seed);
            List<Damage> damages = new List<Damage>();
            int rDamagesLen = r.Next(20, 100);
            for (int i = 0; i < rDamagesLen; i++)
            {
                damages.Add(new Damage(r.Next(1000,10000), DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + i*250));
            }
            DamageSequence damageSequence = new DamageSequence(damages);
            int rPlusTime= r.Next(1000, 1000000);
            DamageModel fakeDamageModel = new DamageModel(
                    new DpsModels.TargetsDictionary.Target(r.Next(0,1000000), mobType, name, 0),
                    false,
                    r.Next(500000,2000000),
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + rPlusTime,
                    DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + rPlusTime + r.Next(5000,30000),
                    damages.Count,
                    r.Next(7000,10000),
                    false,
                    damageSequence,
                    5000
                    );
            entityViewModel.AddEntity(fakeDamageModel);
            listDamageModel.Add(fakeDamageModel);
        }
        */

        public void Dispose()
        {
            this.windowPositionService = null;
            this.windowSizeService = null;

            this.listDamageModel = null;
            DataGrid_Entities.DataContext = null;
            entityViewModel = null;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            for (var vis = sender as Visual; vis != null; vis = VisualTreeHelper.GetParent(vis) as Visual)
            {
                if (vis is DataGridRow)
                {
                    var row = (DataGridRow)vis;
                    int pos = (row.Item as Entity).Id;
                    DamageChartWindow damageChartWindow = new DamageChartWindow();
                    damageChartWindow.Init(listDamageModel[pos].DamageSequence.Clone(), listDamageModel[pos].Target.originalName);
                    if (damageChartWindow.IsStartupPositionUnknown)
                    {
                        damageChartWindow.Top = this.Top;
                        damageChartWindow.Left = this.Left;
                    }
                    damageChartWindow.Show();
                    break;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            windowPositionService.UpdateWindowPosition(name, new Point<double>(this.Left, this.Top));
            windowSizeService.UpdateWindowSize(name, new Size<double>(this.ActualWidth, this.ActualHeight));
            (this as ICloseAllWindowsButMainAware).Unregister_CloseAllWindowsButMainAware();
            Dispose();
        }

        public void CloseAllWindowsButMainAware_HandleEvent(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
